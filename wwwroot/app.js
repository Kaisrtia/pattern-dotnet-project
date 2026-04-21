const statusBox = document.getElementById("status");
const userBadge = document.getElementById("user-badge");
const logoutBtn = document.getElementById("logout-btn");

const loginForm = document.getElementById("login-form");
const transactionForm = document.getElementById("transaction-form");
const createTransactionBtn = document.getElementById("create-transaction-btn");

const transactionTypeEl = document.getElementById("transaction-type");
const amountEl = document.getElementById("amount");

const loadMyBtn = document.getElementById("load-my-btn");
const loadAllBtn = document.getElementById("load-all-btn");
const transactionsBody = document.getElementById("transactions-body");

const idorForm = document.getElementById("idor-form");
const tamperForm = document.getElementById("tamper-form");
const auditResult = document.getElementById("audit-result");

let authState = {
  isAuthenticated: false,
  role: "Guest",
  username: "Guest"
};

function setStatus(message, isError = false) {
  statusBox.textContent = message;
  statusBox.classList.toggle("error", isError);
}

function setAuditMessage(message, tone = "ok") {
  auditResult.textContent = message;
  auditResult.classList.remove("audit-ok", "audit-warn");
  auditResult.classList.add(tone === "ok" ? "audit-ok" : "audit-warn");
}

function setAuthUi() {
  userBadge.textContent = authState.isAuthenticated
    ? authState.username + " | " + authState.role
    : "Guest";

  createTransactionBtn.disabled = !authState.isAuthenticated;
  loadMyBtn.disabled = !authState.isAuthenticated;
  loadAllBtn.disabled = !authState.isAuthenticated || authState.role !== "Admin";
  logoutBtn.disabled = !authState.isAuthenticated;
}

function toggleTransactionFields() {
  const type = transactionTypeEl.value;
  const amount = Number(amountEl.value || 0);
  const isInterbank = type === "interbank";
  const needsSignature = isInterbank && amount > 50000000;

  document.querySelector('[data-field="destination-account"]').classList.toggle("d-none", isInterbank);
  document.querySelector('[data-field="swift-code"]').classList.toggle("d-none", !isInterbank);
  document.querySelector('[data-field="destination-bank"]').classList.toggle("d-none", !isInterbank);
  document.querySelector('[data-field="destination-account-number"]').classList.toggle("d-none", !isInterbank);
  document.querySelector('[data-field="digital-signature"]').classList.toggle("d-none", !needsSignature);
}

async function requestRaw(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    credentials: "include",
    headers: {
      "Content-Type": "application/json",
      ...(options.headers || {})
    }
  });

  let payload = null;
  const contentType = response.headers.get("content-type") || "";
  if (contentType.includes("application/json")) {
    try {
      payload = await response.json();
    } catch {
      payload = null;
    }
  }

  return { response, payload };
}

async function requestJson(path, options = {}) {
  const { response, payload } = await requestRaw(path, options);

  if (!response.ok) {
    const message = payload?.detail || payload?.title || "Request failed";
    throw new Error(message);
  }

  if (response.status === 204) {
    return null;
  }

  return payload;
}

function renderTransactions(items) {
  if (!Array.isArray(items) || items.length === 0) {
    transactionsBody.innerHTML = '<tr><td colspan="7" class="text-secondary">Không có dữ liệu.</td></tr>';
    return;
  }

  transactionsBody.innerHTML = items.map(t => {
    const destination = t.transactionType === "Internal"
      ? (t.destinationAccountId ?? "-")
      : (t.destinationBankName || "-") + " / " + (t.destinationAccountNumber || "-");

    const amount = Number(t.amount).toLocaleString("vi-VN");
    const created = new Date(t.createdAt).toLocaleString("vi-VN");

    return `
      <tr>
        <td class="mono">${t.id}</td>
        <td>${t.transactionType}</td>
        <td>${amount}</td>
        <td>${t.ownerUserId}</td>
        <td>${t.sourceAccountId ?? "-"}</td>
        <td>${destination}</td>
        <td>${created}</td>
      </tr>`;
  }).join("");
}

loginForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  try {
    const payload = {
      username: document.getElementById("username").value,
      password: document.getElementById("password").value
    };

    const data = await requestJson("/api/auth/login", {
      method: "POST",
      body: JSON.stringify(payload)
    });

    authState = {
      isAuthenticated: true,
      username: data.username,
      role: data.role
    };

    setAuthUi();
    setStatus("Đăng nhập thành công.");
    setAuditMessage("Sẵn sàng test IDOR/Tampering.", "ok");
  } catch (error) {
    authState = { isAuthenticated: false, role: "Guest", username: "Guest" };
    setAuthUi();
    setStatus(error.message, true);
  }
});

logoutBtn.addEventListener("click", async () => {
  try {
    await requestJson("/api/auth/logout", { method: "POST" });
  } catch {
    // ignore logout failure to reset local state anyway
  }

  authState = { isAuthenticated: false, role: "Guest", username: "Guest" };
  setAuthUi();
  setStatus("Đã đăng xuất.");
  renderTransactions([]);
});

transactionForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  const type = transactionTypeEl.value;
  const amount = Number(amountEl.value);
  const sourceAccountId = Number(document.getElementById("source-account-id").value);

  try {
    let endpoint = "/api/transactions/internal";
    let payload = {
      amount,
      sourceAccountId,
      destinationAccountId: Number(document.getElementById("destination-account-id").value)
    };

    if (type === "interbank") {
      endpoint = "/api/transactions/interbank";
      payload = {
        amount,
        sourceAccountId,
        swiftCode: document.getElementById("swift-code").value,
        destinationBankName: document.getElementById("destination-bank-name").value,
        destinationAccountNumber: document.getElementById("destination-account-number").value,
        digitalSignature: document.getElementById("digital-signature").value || null
      };
    }

    await requestJson(endpoint, {
      method: "POST",
      body: JSON.stringify(payload)
    });

    setStatus("Tạo giao dịch thành công.");
    await loadMyTransactions();
  } catch (error) {
    setStatus(error.message, true);
  }
});

async function loadMyTransactions() {
  try {
    const data = await requestJson("/api/transactions/me");
    renderTransactions(data);
    setStatus("Đã tải danh sách giao dịch của bạn.");
  } catch (error) {
    setStatus(error.message, true);
  }
}

async function loadAllTransactions() {
  try {
    const data = await requestJson("/api/transactions");
    renderTransactions(data);
    setStatus("Đã tải toàn bộ giao dịch (Admin).");
  } catch (error) {
    setStatus(error.message, true);
  }
}

idorForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  const transactionId = document.getElementById("idor-transaction-id").value.trim();
  if (!transactionId) {
    setAuditMessage("Nhập transaction id để test IDOR.", "warn");
    return;
  }

  const { response, payload } = await requestRaw(`/api/transactions/${transactionId}`);

  if (response.status === 403) {
    setAuditMessage("IDOR blocked đúng yêu cầu: server trả 403 Forbidden.", "ok");
    return;
  }

  if (response.ok) {
    setAuditMessage("Request trả dữ liệu. Nếu đây không phải transaction của bạn thì có rủi ro IDOR.", "warn");
    return;
  }

  const msg = payload?.detail || payload?.title || `HTTP ${response.status}`;
  setAuditMessage(`IDOR test trả ${response.status}: ${msg}`, "warn");
});

tamperForm.addEventListener("submit", async (event) => {
  event.preventDefault();

  const sourceAccountId = Number(document.getElementById("tamper-source-account-id").value);
  const amount = Number(document.getElementById("tamper-amount").value);

  const payload = {
    amount,
    sourceAccountId,
    swiftCode: "DEMO1234",
    destinationBankName: "Tamper Bank",
    destinationAccountNumber: "9876543210",
    digitalSignature: null
  };

  const { response, payload: errorPayload } = await requestRaw("/api/transactions/interbank", {
    method: "POST",
    body: JSON.stringify(payload)
  });

  if (response.status === 400) {
    setAuditMessage("Tampering blocked đúng yêu cầu: server trả 400 Bad Request.", "ok");
    return;
  }

  if (response.ok) {
    setAuditMessage("Tampering test không bị chặn. Hãy kiểm tra lại validation backend.", "warn");
    return;
  }

  const msg = errorPayload?.detail || errorPayload?.title || `HTTP ${response.status}`;
  setAuditMessage(`Tampering test trả ${response.status}: ${msg}`, "warn");
});

loadMyBtn.addEventListener("click", loadMyTransactions);
loadAllBtn.addEventListener("click", loadAllTransactions);

transactionTypeEl.addEventListener("change", toggleTransactionFields);
amountEl.addEventListener("input", toggleTransactionFields);

setAuthUi();
toggleTransactionFields();
setAuditMessage("Đăng nhập để chạy security drill.", "ok");
