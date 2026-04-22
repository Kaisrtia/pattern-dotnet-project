const API_BASE_URL = '/api';
let currentUser = null;

// ============ DOM Elements ============
const loginForm = document.getElementById('loginForm');
const loginSection = document.getElementById('loginSection');
const mainContent = document.getElementById('mainContent');
const logoutBtn = document.getElementById('logoutBtn');
const userDisplay = document.getElementById('userDisplay');
const loadingSpinner = document.getElementById('loadingSpinner');

// Form Elements
const recordTypeSelect = document.getElementById('recordType');
const isDangerousInfectious = document.getElementById('isDangerousInfectious');
const verificationCodeGroup = document.getElementById('verificationCodeGroup');
const inpatientGroup = document.getElementById('inpatientGroup');
const outpatientGroup = document.getElementById('outpatientGroup');
const createRecordForm = document.getElementById('createRecordForm');
const createRecordError = document.getElementById('createRecordError');
const createRecordSuccess = document.getElementById('createRecordSuccess');

// Tab Elements
const tabButtons = document.querySelectorAll('[data-tab]');
const tabMyHistory = document.getElementById('tabMyHistory');
const tabCreateRecord = document.getElementById('tabCreateRecord');
const tabAllRecords = document.getElementById('tabAllRecords');

// ============ Event Listeners ============

loginForm.addEventListener('submit', handleLogin);
logoutBtn.addEventListener('click', handleLogout);
recordTypeSelect.addEventListener('change', handleRecordTypeChange);
isDangerousInfectious.addEventListener('change', handleDangerousInfectiousChange);
createRecordForm.addEventListener('submit', handleCreateRecord);

tabButtons.forEach(btn => {
  btn.addEventListener('click', (e) => {
    const tabName = e.currentTarget.getAttribute('data-tab');
    switchTab(tabName);
  });
});

// ============ Authentication ============

async function handleLogin(e) {
  e.preventDefault();
  
  const username = document.getElementById('username').value;
  const password = document.getElementById('password').value;
  const loginError = document.getElementById('loginError');

  try {
    showLoading();
    
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify({ username, password })
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Đăng nhập thất bại');
    }

    // Parse JWT-like info from response (if needed, otherwise just use cookie)
    currentUser = { username };

    loginError.style.display = 'none';
    loginSection.style.display = 'none';
    mainContent.style.display = 'block';
    
    updateUI();
    loadMyHistory();
  } catch (error) {
    loginError.textContent = '❌ ' + error.message;
    loginError.style.display = 'block';
  } finally {
    hideLoading();
  }
}

async function handleLogout() {
  try {
    showLoading();
    await fetch(`${API_BASE_URL}/auth/logout`, {
      method: 'POST',
      credentials: 'include'
    });

    currentUser = null;
    loginSection.style.display = '';
    mainContent.style.display = 'none';
    loginForm.reset();
    document.getElementById('loginError').style.display = 'none';
  } catch (error) {
    alert('Lỗi khi đăng xuất: ' + error.message);
  } finally {
    hideLoading();
  }
}

// ============ UI Updates ============

function updateUI() {
  userDisplay.textContent = `👤 ${currentUser.username}`;
  userDisplay.style.display = 'inline';
  logoutBtn.style.display = 'block';

  // Check if admin to show admin-only tabs
  checkAdminAccess();
}

async function checkAdminAccess() {
  try {
    // Try to access admin endpoint to check role
    const response = await fetch(`${API_BASE_URL}/medical-records`, {
      method: 'GET',
      credentials: 'include'
    });

    if (response.ok) {
      // User is admin - show createRecord and allRecords tabs
      tabMyHistory.style.display = 'none';
      tabCreateRecord.style.display = 'block';
      tabAllRecords.style.display = 'block';
      
      // Auto-switch to createRecord for admin
      switchTab('createRecord');
    } else if (response.status === 403) {
      // User is not admin - show myHistory tab only
      tabMyHistory.style.display = 'block';
      tabCreateRecord.style.display = 'none';
      tabAllRecords.style.display = 'none';
      
      // Auto-switch to myHistory for user
      switchTab('myHistory');
    }
  } catch (error) {
    console.error('Error checking admin access:', error);
  }
}

function switchTab(tabName) {
  // Hide all tabs
  document.querySelectorAll('.tab-content').forEach(tab => {
    tab.style.display = 'none';
  });

  // Remove active from all buttons
  tabButtons.forEach(btn => {
    btn.classList.remove('active');
  });

  // Show selected tab
  const tabElement = document.getElementById(tabName + 'Tab');
  if (tabElement) {
    tabElement.style.display = 'block';
  }

  // Mark button as active
  document.querySelector(`[data-tab="${tabName}"]`).classList.add('active');

  // Load data for tab
  if (tabName === 'myHistory') {
    loadMyHistory();
  } else if (tabName === 'allRecords') {
    loadAllRecords();
  }
}

// ============ Form Handling ============

function handleRecordTypeChange() {
  const type = recordTypeSelect.value;
  
  inpatientGroup.style.display = type === 'inpatient' ? 'block' : 'none';
  outpatientGroup.style.display = type === 'outpatient' ? 'block' : 'none';

  // Update required attributes
  if (type === 'inpatient') {
    document.getElementById('roomNumber').required = true;
    document.getElementById('bedNumber').required = true;
    document.getElementById('ePrescriptionCode').required = false;
  } else if (type === 'outpatient') {
    document.getElementById('roomNumber').required = false;
    document.getElementById('bedNumber').required = false;
    document.getElementById('ePrescriptionCode').required = true;
  }
}

function handleDangerousInfectiousChange() {
  if (isDangerousInfectious.checked) {
    verificationCodeGroup.style.display = 'block';
    document.getElementById('medicalVerificationCode').required = true;
  } else {
    verificationCodeGroup.style.display = 'none';
    document.getElementById('medicalVerificationCode').required = false;
    document.getElementById('medicalVerificationCode').value = '';
  }
}

async function handleCreateRecord(e) {
  e.preventDefault();

  const recordType = recordTypeSelect.value;
  const recordCode = document.getElementById('recordCode').value;
  const examinationDate = document.getElementById('examinationDate').value;
  const diagnosis = document.getElementById('diagnosis').value;
  const patientId = parseInt(document.getElementById('patientId').value);
  const isDangerous = isDangerousInfectious.checked;
  const verificationCode = document.getElementById('medicalVerificationCode').value || null;

  let payload = {
    recordType,
    recordCode,
    examinationDate,
    diagnosis,
    isDangerousInfectiousDisease: isDangerous,
    medicalVerificationCode: verificationCode,
    patientId
  };

  if (recordType === 'inpatient') {
    payload.roomNumber = document.getElementById('roomNumber').value;
    payload.bedNumber = document.getElementById('bedNumber').value;
  } else if (recordType === 'outpatient') {
    payload.ePrescriptionCode = document.getElementById('ePrescriptionCode').value;
  }

  try {
    showLoading();
    createRecordError.style.display = 'none';
    createRecordSuccess.style.display = 'none';

    const response = await fetch(`${API_BASE_URL}/medical-records`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(payload)
    });

    if (!response.ok) {
      const error = await response.json();
      throw new Error(error.detail || 'Không thể tạo hồ sơ');
    }

    createRecordSuccess.textContent = '✅ Hồ sơ đã được tạo thành công!';
    createRecordSuccess.style.display = 'block';
    createRecordForm.reset();
    inpatientGroup.style.display = 'none';
    outpatientGroup.style.display = 'none';
    verificationCodeGroup.style.display = 'none';

    // Reload lists
    setTimeout(() => {
      loadMyHistory();
      loadAllRecords();
    }, 1500);
  } catch (error) {
    createRecordError.textContent = '❌ ' + error.message;
    createRecordError.style.display = 'block';
  } finally {
    hideLoading();
  }
}

// ============ Data Loading ============

async function loadMyHistory() {
  const myHistoryList = document.getElementById('myHistoryList');
  const myHistoryEmpty = document.getElementById('myHistoryEmpty');

  try {
    showLoading();
    
    const response = await fetch(`${API_BASE_URL}/medical-records/my-history`, {
      credentials: 'include'
    });

    if (!response.ok) {
      throw new Error('Không thể tải lịch sử');
    }

    const records = await response.json();

    if (!records || records.length === 0) {
      myHistoryList.innerHTML = '';
      myHistoryEmpty.style.display = 'block';
    } else {
      myHistoryEmpty.style.display = 'none';
      myHistoryList.innerHTML = records.map(record => createRecordHTML(record)).join('');
    }
  } catch (error) {
    myHistoryList.innerHTML = `<div class="alert alert-danger">Lỗi: ${error.message}</div>`;
  } finally {
    hideLoading();
  }
}

async function loadAllRecords() {
  const allRecordsList = document.getElementById('allRecordsList');
  const allRecordsEmpty = document.getElementById('allRecordsEmpty');

  try {
    showLoading();
    
    const response = await fetch(`${API_BASE_URL}/medical-records`, {
      credentials: 'include'
    });

    if (!response.ok) {
      throw new Error('Không thể tải danh sách hồ sơ');
    }

    const records = await response.json();

    if (!records || records.length === 0) {
      allRecordsList.innerHTML = '';
      allRecordsEmpty.style.display = 'block';
    } else {
      allRecordsEmpty.style.display = 'none';
      allRecordsList.innerHTML = records.map(record => createRecordHTML(record)).join('');
    }
  } catch (error) {
    allRecordsList.innerHTML = `<div class="alert alert-danger">Lỗi: ${error.message}</div>`;
  } finally {
    hideLoading();
  }
}

// ============ HTML Generators ============

function createRecordHTML(record) {
  const typeLabel = record.recordType === 'Inpatient' ? 'Nội Trú' : 'Ngoại Trú';
  const typeClass = record.recordType === 'Inpatient' ? 'inpatient' : 'outpatient';
  const examinationDateObj = new Date(record.examinationDate + 'T00:00:00');
  const examinationDateFormatted = examinationDateObj.toLocaleDateString('vi-VN');
  
  let specialInfo = '';
  if (record.recordType === 'Inpatient') {
    specialInfo = `
      <div class="record-detail">
        <strong>Số Phòng:</strong> ${record.roomNumber}
      </div>
      <div class="record-detail">
        <strong>Số Giường:</strong> ${record.bedNumber}
      </div>
    `;
  } else {
    specialInfo = `
      <div class="record-detail">
        <strong>Mã Toa Thuốc:</strong> ${record.ePrescriptionCode}
      </div>
    `;
  }

  let dangerousAlert = '';
  if (record.isDangerousInfectiousDisease) {
    dangerousAlert = `
      <div class="record-dangerous">
        ⚠️ Bệnh truyền nhiễm nguy hiểm - Mã xác thực: ${record.medicalVerificationCode}
      </div>
    `;
  }

  return `
    <div class="record-item">
      <div class="record-code">📋 ${record.recordCode}</div>
      <span class="record-type ${typeClass}">${typeLabel}</span>
      <div class="record-detail">
        <strong>Chẩn Đoán:</strong> ${record.diagnosis}
      </div>
      <div class="record-detail">
        <strong>Bệnh Nhân ID:</strong> ${record.patientId}
      </div>
      ${specialInfo}
      ${dangerousAlert}
      <div class="record-date">
        📅 Ngày Khám: ${examinationDateFormatted}
      </div>
    </div>
  `;
}

// ============ Utility Functions ============

function showLoading() {
  loadingSpinner.style.display = 'block';
}

function hideLoading() {
  loadingSpinner.style.display = 'none';
}

// ============ Initialize ============

// Set today as max date for examination date
document.getElementById('examinationDate').max = new Date().toISOString().split('T')[0];

console.log('✅ Ứng dụng đã sẵn sàng');
