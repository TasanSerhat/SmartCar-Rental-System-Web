// API_URL is defined in api.js
let isEditing = false;

function getHeaders() {
    const token = localStorage.getItem('token');
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
    };
}

function checkAuth(response) {
    if (response.status === 401) {
        alert('Session expired. Please login again.');
        window.location.href = '../login.html';
        return false;
    }
    return true;
}

document.addEventListener('DOMContentLoaded', () => {
    loadVehicles();


    // Initial Load
    loadVehicles();

    // Tab Switching
    window.switchTab = function (tabName) {
        // Update Buttons
        document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));
        event.target.classList.add('active');

        // Update Sections
        // Update Sections - Explicitly hide ALL sections
        // Note: Using explicit IDs because querySelectorAll('.section') isn't set up
        const sections = ['vehicles-section', 'reservations-section', 'rentals-section', 'customers-section', 'employees-section', 'branches-section'];
        sections.forEach(id => {
            const el = document.getElementById(id);
            if (el) el.style.display = 'none';
        });

        if (tabName === 'vehicles') {
            document.getElementById('vehicles-section').style.display = 'block';
        } else if (tabName === 'reservations') {
            document.getElementById('reservations-section').style.display = 'block';
            loadReservations();
        } else if (tabName === 'rentals') {
            document.getElementById('rentals-section').style.display = 'block';
            loadRentalHistory();
        } else if (tabName === 'customers') {
            document.getElementById('customers-section').style.display = 'block';
            loadCustomers();
        } else if (tabName === 'employees') {
            document.getElementById('employees-section').style.display = 'block';
            loadEmployees();
        } else if (tabName === 'branches') {
            document.getElementById('branches-section').style.display = 'block';
            loadBranches();
        }
    }

    document.getElementById('vehicleForm').addEventListener('submit', handleFormSubmit);
    document.getElementById('customerForm').addEventListener('submit', handleCustomerSubmit);
    document.getElementById('employeeForm').addEventListener('submit', handleEmployeeSubmit);
    document.getElementById('branchForm').addEventListener('submit', handleBranchSubmit);
});

// --- RENTAL VIEW TOGGLE ---
window.toggleRentalView = function (view) {
    const activeView = document.getElementById('active-rentals-view');
    const pastView = document.getElementById('past-rentals-view');
    const btnActive = document.getElementById('btn-view-active');
    const btnPast = document.getElementById('btn-view-past');

    if (view === 'active') {
        activeView.style.display = 'block';
        pastView.style.display = 'none';
        btnActive.style.background = 'var(--primary-color)';
        btnActive.style.color = 'white';
        btnActive.style.border = 'none';
        btnPast.style.background = 'transparent';
        btnPast.style.color = 'var(--text-dim)';
        btnPast.style.border = '1px solid var(--text-dim)';
    } else {
        activeView.style.display = 'none';
        pastView.style.display = 'block';
        btnPast.style.background = 'var(--primary-color)';
        btnPast.style.color = 'white';
        btnPast.style.border = 'none';
        btnActive.style.background = 'transparent';
        btnActive.style.color = 'var(--text-dim)';
        btnActive.style.border = '1px solid var(--text-dim)';
    }
}

// --- RESERVATIONS ---
async function loadReservations() {
    try {
        const response = await fetch(`${API_URL}/Reservations/getdetails`, { headers: getHeaders() });
        const result = await response.json();
        const reservations = result.data ? result.data : result;

        const tbody = document.getElementById('reservation-table-body');
        tbody.innerHTML = '';

        reservations.forEach(r => {
            const tr = document.createElement('tr');

            // Only show Pending/Confirmed reservations that are NOT yet Rented/Completed
            // If we filter, we might hide 'Rented' ones. Let's show ALL but change Actions.

            const isRented = r.status === 'Rented';
            const isCompleted = r.status === 'Completed';
            const isCancelled = r.status === 'Cancelled';
            const canRent = !isRented && !isCompleted && !isCancelled;

            tr.innerHTML = `
                <td>#${r.reservationId}</td>
                <td>${r.customerName}</td>
                <td><span style="color:var(--primary-color); font-weight:bold;">${r.vehiclePlate}</span></td>
                <td>${new Date(r.startDate).toLocaleDateString()}</td>
                <td>${new Date(r.endDate).toLocaleDateString()}</td>
                <td>$${r.totalPrice}</td>
                <td><span class="status-badge status-${r.status.toLowerCase()}">${r.status}</span></td>
                <td>
                    ${canRent ? `<button class="action-btn" style="background:#f59e0b; color:black;" onclick="startRental(${r.reservationId}, ${r.totalPrice})">Start Rental</button>` : '-'}
                </td>
            `;
            tbody.appendChild(tr);
        });

    } catch (error) {
        console.error('Error loading reservations:', error);
    }
}

// --- RENTALS (ACTIVE & PAST) ---
async function loadRentalHistory() {
    try {
        console.log("Loading Rental History...");
        // Use Reservation Details to display rental info, filtered by Status
        // Note: Ideally we link Rental Entity to Reservation to get RentalId.
        // For 'End Rental', we need RentalId. 
        // Problem: getdetails returns ReservationId, NOT RentalId.
        // Solution: We need RentalId to call EndRental.
        // Option 1: Fetch Rentals (getAll) -> Match with Reservation details.
        // Option 2: Add GetRentalIdByReservationId? 
        // Option 3: EndRental by ReservationId? (Change Logic?)
        // Let's Fetch Rentals too to perform the mapping.

        const [reservationsRes, rentalsRes] = await Promise.all([
            fetch(`${API_URL}/Reservations/getdetails`, { headers: getHeaders() }),
            fetch(`${API_URL}/Rentals/getall`, { headers: getHeaders() })
        ]);

        const resData = await reservationsRes.json();
        const reservations = resData.data ? resData.data : resData;

        const rentData = await rentalsRes.json();
        // Check both formats for rentals too
        const rentals = rentData.data ? rentData.data : (Array.isArray(rentData) ? rentData : []);

        console.log("Reservations:", reservations);
        console.log("Rentals:", rentals);

        // Map ReservationId -> RentalId
        // Handle case sensitivity (API might return rentalId or RentalId)
        const rentalMap = {};
        rentals.forEach(r => {
            const rId = r.rentalId || r.RentalId;
            const resId = r.reservationId || r.ReservationId;
            if (resId) rentalMap[resId] = rId;
        });

        console.log("Rental Map:", rentalMap);

        const activeBody = document.getElementById('active-rentals-table-body');
        const pastBody = document.getElementById('past-rentals-table-body');
        activeBody.innerHTML = '';
        pastBody.innerHTML = '';

        reservations.forEach(r => {
            const rentalId = rentalMap[r.reservationId]; // reservationId from getdetails DTO

            // Check status (Case insensitive safety)
            const status = (r.status || "").toLowerCase();

            if (status === 'rented') {
                // Active
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>#${r.reservationId}</td>
                    <td>${r.customerName}</td>
                    <td><span style="color:var(--primary-color); font-weight:bold;">${r.vehiclePlate}</span></td>
                    <td>${new Date(r.startDate).toLocaleDateString()}</td>
                    <td>${new Date(r.endDate).toLocaleDateString()}</td>
                    <td>$${r.totalPrice}</td>
                    <td>
                        <button class="action-btn" style="background:#ef4444; color:white;" onclick="endRental(${rentalId})">End Rental</button>
                    </td>
                `;
                activeBody.appendChild(tr);

            } else if (status === 'completed') {
                // Past
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>#${r.reservationId}</td>
                    <td>${r.customerName}</td>
                    <td>${r.vehiclePlate}</td>
                    <td>${new Date(r.startDate).toLocaleDateString()}</td>
                    <td>${new Date(r.endDate).toLocaleDateString()}</td>
                    <td>$${r.totalPrice}</td>
                    <td><span class="status-badge status-completed">Completed</span></td>
                `;
                pastBody.appendChild(tr);
            }
        });

    } catch (e) {
        console.error("Error in loadRentalHistory:", e);
    }
}

async function startRental(reservationId, totalPrice) {
    if (!confirm('Start rental for this reservation?')) return;

    const rental = {
        reservationId: reservationId,
        paymentId: 0,
        totalDays: 3,
        totalCost: totalPrice
    };

    try {
        const response = await fetch(`${API_URL}/Rentals/add`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify(rental)
        });

        if (response.ok) {
            alert('Rental Started!');
            loadReservations(); // Refresh
        } else {
            const err = await response.text();
            alert('Failed: ' + err);
        }
    } catch (e) {
        console.error(e);
    }
}

async function endRental(rentalId) {
    if (!rentalId) {
        alert("Error: Rental ID not found or mapped.");
        return;
    }
    if (!confirm('End this rental? The vehicle will be marked as Available.')) return;

    try {
        const response = await fetch(`${API_URL}/Rentals/endrental?rentalId=${rentalId}`, {
            method: 'POST',
            headers: getHeaders()
        });

        if (response.ok) {
            alert('Rental Ended Successfully!');
            loadRentalHistory(); // Refresh lists
        } else {
            const err = await response.text();
            alert('Failed: ' + err);
        }

    } catch (e) {
        console.error(e);
    }
}

// --- CUSTOMERS ---
async function loadCustomers() {
    try {
        const response = await fetch(`${API_URL}/Customers/getall`, { headers: getHeaders() });
        if (!checkAuth(response)) return;
        const result = await response.json();
        const customers = result.data ? result.data : result;

        const tbody = document.getElementById('customer-table-body');
        tbody.innerHTML = '';

        customers.forEach(c => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>#${c.id}</td>
                <td>${c.firstName} ${c.lastName}</td>
                <td>${c.email}</td>
                <td>${c.licenseNo || '-'}</td>
                <td>${c.phone || '-'}</td>
                <td>
                    <button class="action-btn btn-edit" onclick="editCustomer(${c.id})">Edit</button>
                    <button class="action-btn btn-delete" onclick="deleteCustomer(${c.id})">Delete</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (error) {
        console.error('Error loading customers:', error);
    }
}

let isEditingCustomer = false;

function openCustomerModal() {
    isEditingCustomer = false;
    document.getElementById('customerForm').reset();
    document.getElementById('customerId').value = '';
    document.getElementById('customerModalTitle').innerText = 'Add New Customer';
    document.getElementById('customerModal').style.display = 'block';
}

async function editCustomer(id) {
    try {
        const response = await fetch(`${API_URL}/Customers/getbyid?id=${id}`, { headers: getHeaders() });
        const result = await response.json();
        const customer = result.data || result;

        isEditingCustomer = true;
        document.getElementById('customerModalTitle').innerText = 'Edit Customer';
        document.getElementById('customerId').value = customer.id;
        document.getElementById('firstName').value = customer.firstName;
        document.getElementById('lastName').value = customer.lastName;
        document.getElementById('email').value = customer.email;
        document.getElementById('phone').value = customer.phone;
        document.getElementById('address').value = customer.address || '';
        document.getElementById('licenseNo').value = customer.licenseNo;

        document.getElementById('customerModal').style.display = 'block';
    } catch (error) {
        console.error('Error fetching customer:', error);
        alert('Could not load customer details');
    }
}

// Close Customer Modal logic
document.querySelector('.close-customer-modal').onclick = function () {
    document.getElementById('customerModal').style.display = 'none';
}
document.getElementById('btnCancelCustomer').onclick = function () {
    document.getElementById('customerModal').style.display = 'none';
}

async function handleCustomerSubmit(e) {
    e.preventDefault();

    const customerId = document.getElementById('customerId').value;
    const customer = {
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value,
        licenseNo: document.getElementById('licenseNo').value,
        address: document.getElementById('address').value,
        status: true
    };

    if (isEditingCustomer) {
        customer.id = parseInt(customerId);
    }

    // Always send dummy password data to satisfy validation (Backend will ignore/restore for updates)
    customer.passwordHash = "ZHVtbXloYXNo";
    customer.passwordSalt = "ZHVtbXlzYWx0";

    const endpoint = isEditingCustomer ? 'update' : 'add';

    try {
        const response = await fetch(`${API_URL}/Customers/${endpoint}`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify(customer)
        });

        if (response.ok) {
            alert(isEditingCustomer ? 'Customer updated!' : 'Customer saved!');
            document.getElementById('customerModal').style.display = 'none';
            loadCustomers();
        } else {
            const err = await response.text();
            alert('Error saving customer: ' + err);
        }
    } catch (error) {
        console.error(error);
        alert('Error saving customer');
    }
}

async function deleteCustomer(id) {
    if (!confirm('Delete this customer?')) return;

    try {
        const response = await fetch(`${API_URL}/Customers/delete?id=${id}`, {
            method: 'POST',
            headers: getHeaders()
        });
        if (!checkAuth(response)) return;

        if (response.ok) {
            loadCustomers();
        } else {
            const err = await response.text();
            alert('Failed to delete: ' + err);
        }
    } catch (e) { console.error(e); }
}

// --- VEHICLES ---
async function loadVehicles() {
    try {
        const response = await fetch(`${API_URL}/Vehicles/getall`, { headers: getHeaders() });
        const result = await response.json();

        // Note: The structure depends on the API return type.
        // Assuming result is [ { vehicleId: 1, ... }, ... ] OR { data: [...], success: true }
        // Since we refactored to IResult, it depends on whether the serialized JSON contains Data property.
        // Usually, manual serialization of IDataResult returns { "data": [...], "success": true, "message": null }

        let vehicles = [];
        if (Array.isArray(result)) {
            vehicles = result;
        } else if (result.data) {
            vehicles = result.data;
        }

        renderTable(vehicles);
    } catch (error) {
        console.error('Error loading vehicles:', error);
        alert('Failed to load vehicles');
    }
}

function renderTable(vehicles) {
    const tbody = document.getElementById('vehicle-table-body');
    tbody.innerHTML = '';

    vehicles.forEach(vehicle => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>#${vehicle.vehicleId}</td>
            <td><span style="font-weight: 600; color: white;">${vehicle.plateNo}</span></td>
            <td>${getColorName(vehicle.colorId)}</td>
            <td>$${vehicle.model ? vehicle.model.pricePerDay : '-'}</td>
            <td>${vehicle.model ? vehicle.model.year : '-'}</td>
            <td>
                <button class="action-btn btn-edit" onclick="editVehicle(${vehicle.vehicleId})">Edit</button>
                <button class="action-btn btn-delete" onclick="deleteVehicle(${vehicle.vehicleId})">Delete</button>
                <button class="action-btn" style="background:#2ecc71; color:white; margin-left:5px;" onclick="openImageModal(${vehicle.vehicleId})">Img</button>
            </td>
        `;
        tbody.appendChild(tr);
    });
}

function getColorName(id) {
    // Mock color mapping
    const colors = { 1: 'Red', 2: 'Blue', 3: 'White', 4: 'Black' };
    return colors[id] || 'Unknown';
}


async function loadMasterData() {
    try {
        // Load Models
        const modelsRes = await fetch(`${API_URL}/VehicleModels/getall`, { headers: getHeaders() });
        const modelsData = await modelsRes.json();
        const models = modelsData.data || [];
        const modelSelect = document.getElementById('modelId');
        // Preserve default option
        modelSelect.innerHTML = '<option value="">Select a Model</option>';
        models.forEach(m => {
            modelSelect.innerHTML += `<option value="${m.modelId}">${m.brand} ${m.modelName} (${m.year})</option>`;
        });

        // Load Branches
        const branchesRes = await fetch(`${API_URL}/Branches/getall`, { headers: getHeaders() });
        const branchesData = await branchesRes.json();
        const branches = branchesData.data || [];
        const branchSelect = document.getElementById('branchId');
        branchSelect.innerHTML = '<option value="">Select a Branch</option>';
        branches.forEach(b => {
            branchSelect.innerHTML += `<option value="${b.branchId}">${b.branchName} (${b.city})</option>`;
        });

        // Load Colors
        const colorsRes = await fetch(`${API_URL}/Colors/getall`, { headers: getHeaders() });
        const colorsData = await colorsRes.json();
        const colors = colorsData.data || [];
        const colorSelect = document.getElementById('colorId');
        colorSelect.innerHTML = '<option value="">Select a Color</option>';
        colors.forEach(c => {
            colorSelect.innerHTML += `<option value="${c.colorId}">${c.colorName}</option>`;
        });

    } catch (e) {
        console.error('Error loading master data:', e);
    }
}

function openModal() {
    isEditing = false;
    document.getElementById('vehicleForm').reset();
    document.getElementById('modalTitle').textContent = 'Add New Vehicle';
    document.getElementById('vehicleId').value = '';
    document.getElementById('vehicleModal').classList.add('active');
    loadMasterData(); // Call this to populate dropdowns
}

function closeModal() {
    document.getElementById('vehicleModal').classList.remove('active');
}

async function editVehicle(id) {
    try {
        const response = await fetch(`${API_URL}/Vehicles/getbyid?id=${id}`, { headers: getHeaders() });
        const result = await response.json();
        /* 
           API returns `DataResult<Vehicle>`, so usually we get the object inside `data` property 
           OR simply the object depending on Controller serialization. 
           Let's handle both.
        */
        const vehicle = result.data || result;

        isEditing = true;
        document.getElementById('modalTitle').textContent = 'Edit Vehicle';
        document.getElementById('vehicleId').value = vehicle.vehicleId;
        document.getElementById('plateNo').value = vehicle.plateNo;
        document.getElementById('modelId').value = vehicle.modelId;
        document.getElementById('branchId').value = vehicle.branchId;
        document.getElementById('colorId').value = vehicle.colorId;
        // document.getElementById('modelYear').value = vehicle.modelYear;
        // document.getElementById('dailyPrice').value = vehicle.dailyPrice;
        document.getElementById('description').value = vehicle.description || '';

        document.getElementById('vehicleModal').classList.add('active');
    } catch (error) {
        console.error('Error fetching vehicle:', error);
        alert('Could not load vehicle details');
    }
}

async function deleteVehicle(id) {
    if (!confirm('Are you sure you want to delete this vehicle?')) return;

    try {
        // We need to send a vehicle object with ID for the Delete method as currently implemented
        // Or refactor controller to accept ID. 
        // Controller: Delete(Vehicle vehicle) -> needs Post Body.
        // Let's create a minimal object. 
        const response = await fetch(`${API_URL}/Vehicles/delete?id=${id}&t=${new Date().getTime()}`, {
            method: 'POST',
            headers: getHeaders()
        });

        if (response.ok) {
            alert('Vehicle deleted!');
            loadVehicles();
        } else {
            alert('Failed to delete vehicle');
        }
    } catch (error) {
        console.error('Error deleting:', error);
    }
}

async function handleFormSubmit(e) {
    e.preventDefault();

    const vehicle = {
        vehicleId: document.getElementById('vehicleId').value ? parseInt(document.getElementById('vehicleId').value) : 0,
        plateNo: document.getElementById('plateNo').value,
        modelId: parseInt(document.getElementById('modelId').value),
        branchId: parseInt(document.getElementById('branchId').value),
        colorId: parseInt(document.getElementById('colorId').value),
        // modelYear & dailyPrice moved to Model
        description: document.getElementById('description').value,
        kilometer: 0,
        fuelType: "Electric",
        status: "Available"
    };

    const endpoint = isEditing ? 'update' : 'add';

    try {
        const response = await fetch(`${API_URL}/Vehicles/${endpoint}`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify(vehicle)
        });

        if (response.ok) {
            alert(isEditing ? 'Vehicle updated!' : 'Vehicle added!');
            closeModal();
            loadVehicles();
        } else {
            const err = await response.text();
            alert('Operation failed: ' + err);
        }
    } catch (error) {
        console.error('Error saving:', error);
        alert('Error saving vehicle');
    }
}

// --- IMAGE UPLOAD LOGIC ---
function openImageModal(vehicleId) {
    document.getElementById('imageVehicleId').value = vehicleId;
    document.getElementById('imageForm').reset();
    document.getElementById('imageModal').classList.add('active');
}

function closeImageModal() {
    document.getElementById('imageModal').classList.remove('active');
}

document.getElementById('imageForm').onsubmit = async function (e) {
    e.preventDefault();

    const fileInput = document.getElementById('imageFile');
    const vehicleId = document.getElementById('imageVehicleId').value;

    if (fileInput.files.length === 0) return;

    const formData = new FormData();
    formData.append('file', fileInput.files[0]);
    formData.append('vehicleId', vehicleId);

    try {
        const token = localStorage.getItem('token');
        const response = await fetch(`${API_URL}/VehicleImages/add`, {
            method: 'POST',
            headers: {
                'Authorization': `Bearer ${token}`
                // Don't set Content-Type, fetch sets it to multipart/form-data with boundary automatically
            },
            body: formData
        });

        if (response.ok) {
            alert('Image uploaded successfully!');
            closeImageModal();
            loadVehicles(); // Refresh to potentially show image status
        } else {
            const err = await response.text();
            alert('Upload failed: ' + err);
        }
    } catch (error) {
        console.error(error);
        alert('Error uploading image');
    }
}

// --- EMPLOYEES ---
async function loadEmployees() {
    console.log("loadEmployees called");
    try {
        const response = await fetch(`${API_URL}/Employees/getall`, { headers: getHeaders() });
        const result = await response.json();
        const employees = result.data || result;

        const tbody = document.getElementById('employee-table-body');
        tbody.innerHTML = '';

        employees.forEach(e => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>#${e.id}</td>
                <td>${e.firstName} ${e.lastName}</td>
                <td>${e.role}</td>
                <td>#${e.branchId}</td>
                <td>${e.email}</td>
                <td>$${e.salary}</td>
                <td>
                    <button class='action-btn btn-delete' onclick='deleteEmployee(${e.id})'>Delete</button>
                </td>
            `;
            tbody.appendChild(tr);
        });
    } catch (e) { console.error(e); }
}

function openEmployeeModal() {
    document.getElementById('employeeForm').reset();
    document.getElementById('employeeId').value = '';
    document.getElementById('employeeModal').style.display = 'block';
    loadMasterDataForEmployee();
}

async function loadMasterDataForEmployee() {
    // Re-using logic to load branches into empBranchId
    try {
        const branchesRes = await fetch(`${API_URL}/Branches/getall`, { headers: getHeaders() });
        const branchesData = await branchesRes.json();
        const branches = branchesData.data || [];
        const select = document.getElementById('empBranchId');
        select.innerHTML = '<option value="">Select Branch</option>';
        branches.forEach(b => {
            select.innerHTML += `<option value='${b.branchId}'>${b.branchName}</option>`;
        });
    } catch (e) { console.error(e); }
}

async function handleEmployeeSubmit(e) {
    e.preventDefault();
    const emp = {
        firstName: document.getElementById('empFirstName').value,
        lastName: document.getElementById('empLastName').value,
        email: document.getElementById('empEmail').value,
        role: document.getElementById('empRole').value,
        branchId: parseInt(document.getElementById('empBranchId').value),
        salary: parseFloat(document.getElementById('empSalary').value),
        status: true,
        passwordHash: 'ZHVtbXloYXNo', // Dummy
        passwordSalt: 'ZHVtbXlzYWx0'  // Dummy
    };

    try {
        const response = await fetch(`${API_URL}/Employees/add`, {
            method: 'POST', headers: getHeaders(), body: JSON.stringify(emp)
        });
        if (response.ok) {
            alert('Employee Added');
            document.getElementById('employeeModal').style.display = 'none';
            loadEmployees();
        } else {
            alert('Error: ' + await response.text());
        }
    } catch (e) { console.error(e); }
}

async function deleteEmployee(id) {
    if (!confirm('Delete this employee?')) return;
    try {
        // Send object with ID for delete
        const response = await fetch(`${API_URL}/Employees/delete`, {
            method: 'POST', headers: getHeaders(),
            body: JSON.stringify({ id: id })
        });
        if (response.ok) loadEmployees();
        else alert('Failed: ' + await response.text());
    } catch (e) { console.error(e); }
}

// --- BRANCHES ---
async function loadBranches() {
    try {
        const response = await fetch(`${API_URL}/Branches/getall`, { headers: getHeaders() });
        const result = await response.json();
        const branches = result.data || result;
        const tbody = document.getElementById('branch-table-body');
        tbody.innerHTML = '';
        branches.forEach(b => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>#${b.branchId}</td>
                <td>${b.branchName}</td>
                <td>${b.city}</td>
                <td>${b.address || '-'}</td>
                <td>${b.contactEmail || '-'}</td>
                <td><button class='action-btn btn-delete' onclick='alert("Delete not impl for branch MVP")'>Delete</button></td>
            `;
            tbody.appendChild(tr);
        });
    } catch (e) { console.error(e); }
}

function openBranchModal() {
    document.getElementById('branchForm').reset();
    document.getElementById('branchModal').style.display = 'block';
}

async function handleBranchSubmit(e) {
    e.preventDefault();
    const branch = {
        branchName: document.getElementById('branchName').value,
        city: document.getElementById('branchCity').value,
        address: document.getElementById('branchAddress').value,
        contactEmail: document.getElementById('branchEmail').value
    };

    try {
        const response = await fetch(`${API_URL}/Branches/add`, {
            method: 'POST', headers: getHeaders(), body: JSON.stringify(branch)
        });
        if (response.ok) {
            alert('Branch Added');
            document.getElementById('branchModal').style.display = 'none';
            loadBranches();
        } else {
            alert('Error: ' + await response.text());
        }
    } catch (e) { console.error(e); }
}
