// API_URL is defined in api.js
const modal = document.getElementById('rental-modal');
const closeModal = document.querySelector('.close-modal');
const rentalForm = document.getElementById('rental-form');
let currentDailyPrice = 0;

function updateNavigation() {
    console.log("updateNavigation called");
    const navMenu = document.getElementById('nav-menu');
    if (!navMenu) {
        console.error("nav-menu element not found!");
        return;
    }

    const token = localStorage.getItem('token');
    console.log("Token present:", !!token);

    let html = '<a href="index.html">Fleet</a>';

    if (token) {
        // Assume admin if token exists for now (or parse JWT if needed)
        // Ideally: parseJwt(token).role or email
        // Logic: Standard user sees "Vehicles". Admin sees "Admin Panel".

        // Simple JWT parse to check email/role
        try {
            const base64Url = token.split('.')[1];
            const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
            const payload = JSON.parse(window.atob(base64));

            // Check for admin email or role
            // Claims keys might vary. Look for email or role.
            const email = payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];

            console.log("User Email:", email);

            if (email && email.includes('admin')) {
                html += '<a href="admin.html">Admin Panel</a>';
            }
            html += '<a href="profile.html">My Account</a>';
        } catch (e) {
            console.error("Token parse error:", e);
        }

        html += '<a href="#" onclick="logoutUser()">Logout</a>';
    } else {
        html += '<a href="login.html">Login</a>';
    }

    console.log("Generated HTML:", html);
    navMenu.innerHTML = html;
}

function logoutUser() {
    localStorage.removeItem('token');
    window.location.reload();
}

document.addEventListener('DOMContentLoaded', updateNavigation);

async function fetchVehicles() {
    const listElement = document.getElementById('vehicle-list');
    if (!listElement) return; // Exit if not on homepage

    try {
        const response = await fetch(`${API_URL}/Vehicles/getall`);
        if (!response.ok) throw new Error('Network response was not ok');

        const result = await response.json();
        // Handle DataResult wrapper if present
        const vehicles = result.data ? result.data : result;

        listElement.innerHTML = '';

        if (vehicles.length === 0) {
            listElement.innerHTML = '<div class="loading-spinner">No vehicles found in the fleet.</div>';
            return;
        }

        vehicles.forEach(vehicle => {
            const card = document.createElement('div');
            card.className = 'vehicle-card';

            // Image Carousel Logic
            let imageContent = '';
            const images = vehicle.vehicleImages || [];

            if (images.length === 0) {
                imageContent = `<div class="card-image-container" style="height:200px; overflow:hidden;">
                    <img src="assets/images/placeholder-car.png" alt="${vehicle.plateNo}" style="width:100%; height:100%; object-fit:cover;">
                 </div>`;
            } else if (images.length === 1) {
                imageContent = `<div class="card-image-container" style="height:200px; overflow:hidden;">
                    <img src="${"http://localhost:5242" + images[0].imagePath}" alt="${vehicle.plateNo}" style="width:100%; height:100%; object-fit:cover;">
                 </div>`;
            } else {
                // Carousel Mode
                let slides = '';
                images.forEach((img, index) => {
                    const isActive = index === 0 ? 'active' : '';
                    slides += `<img src="${"http://localhost:5242" + img.imagePath}" class="carousel-slide ${isActive}" alt="${vehicle.plateNo}" data-index="${index}">`;
                });

                imageContent = `
                <div class="carousel-container" id="carousel-${vehicle.vehicleId}">
                    ${slides}
                    <button class="carousel-btn prev" onclick="prevGeneric(${vehicle.vehicleId})">&#10094;</button>
                    <button class="carousel-btn next" onclick="nextGeneric(${vehicle.vehicleId})">&#10095;</button>
                </div>`;
            }

            card.innerHTML = `
                ${imageContent}
                <div class="card-content">
                    <h3 class="card-title">${vehicle.model ? vehicle.model.brand + ' ' + vehicle.model.modelName : vehicle.plateNo}</h3>
                    <div class="card-info">
                        <span>Year: ${vehicle.model ? vehicle.model.year : 'N/A'}</span>
                        <span>Fuel: ${vehicle.fuelType}</span>
                    </div>
                    <div class="card-info">
                        <span>${vehicle.description || 'N/A'}</span>
                        <span>${vehicle.status}</span>
                    </div>
                    <span class="price-tag">$${vehicle.model ? vehicle.model.pricePerDay : 0} / day</span>
                    <button class="btn-rent" onclick="openRentalModal(${vehicle.vehicleId}, ${vehicle.model ? vehicle.model.pricePerDay : 0}, '${vehicle.model ? vehicle.model.brand + ' ' + vehicle.model.modelName : 'Vehicle'}', '${vehicle.plateNo}')" ${vehicle.status !== 'Available' ? 'disabled style="opacity:0.5;cursor:not-allowed"' : ''}>Rent Now</button>
                </div>
            `;
            listElement.appendChild(card);
        });

    } catch (error) {
        console.error('Error fetching vehicles:', error);
        listElement.innerHTML = `<div class="loading-spinner" style="color:red">Error loading data. Is the backend running?</div>`;
    }
}

// Carousel Navigation Helpers
window.prevGeneric = function (vehicleId) {
    const container = document.getElementById(`carousel-${vehicleId}`);
    const slides = container.querySelectorAll('.carousel-slide');
    let activeIndex = 0;

    slides.forEach((slide, index) => {
        if (slide.classList.contains('active')) {
            activeIndex = index;
            slide.classList.remove('active');
        }
    });

    let newIndex = activeIndex - 1;
    if (newIndex < 0) newIndex = slides.length - 1;

    slides[newIndex].classList.add('active');
}

window.nextGeneric = function (vehicleId) {
    const container = document.getElementById(`carousel-${vehicleId}`);
    const slides = container.querySelectorAll('.carousel-slide');
    let activeIndex = 0;

    slides.forEach((slide, index) => {
        if (slide.classList.contains('active')) {
            activeIndex = index;
            slide.classList.remove('active');
        }
    });

    let newIndex = activeIndex + 1;
    if (newIndex >= slides.length) newIndex = 0;

    slides[newIndex].classList.add('active');
}

// Modal Logic
// --- Payment Wizard & Rental Logic ---
let stepDates, stepPayment, btnNext, btnBack, pricePreview;
let currentStep = 1;

function updateModalSteps() {
    stepDates = document.getElementById('step-dates');
    stepPayment = document.getElementById('step-payment');

    // Ensure elements exist
    if (!stepDates || !stepPayment) return;

    if (currentStep === 1) {
        stepDates.style.display = 'block';
        stepPayment.style.display = 'none';
        if (document.getElementById('price-preview')) document.getElementById('price-preview').style.display = 'none';
    } else {
        stepDates.style.display = 'none';
        stepPayment.style.display = 'block';
        if (document.getElementById('price-preview')) document.getElementById('price-preview').style.display = 'block';
    }
}

document.addEventListener('DOMContentLoaded', () => {
    stepDates = document.getElementById('step-dates');
    stepPayment = document.getElementById('step-payment');
    btnNext = document.getElementById('btn-next-payment');
    btnBack = document.getElementById('btn-back-dates');
    pricePreview = document.getElementById('price-preview');

    // Attach listeners for dates to auto-calculate
    document.getElementById('start-date').addEventListener('change', calculateTotal);
    document.getElementById('end-date').addEventListener('change', calculateTotal);

    // Close Modal Logic
    document.querySelector('.close-modal').addEventListener('click', () => {
        document.getElementById('rental-modal').style.display = 'none';
    });

    if (btnNext) {
        btnNext.addEventListener('click', function (e) {
            e.preventDefault();
            const startDate = document.getElementById('start-date').value;
            const endDate = document.getElementById('end-date').value;

            if (!startDate || !endDate) {
                alert("Please select rental dates first.");
                return;
            }

            if (new Date(startDate) > new Date(endDate)) {
                alert("End date must be after start date.");
                return;
            }

            calculateTotal(); // Ensure fresh calc

            const total = document.getElementById('summary-total').textContent;
            document.getElementById('total-price-val').innerText = total;

            // Switch to Payment
            currentStep = 2;
            updateModalSteps();
        });
    }

    if (btnBack) {
        btnBack.onclick = function () {
            currentStep = 1;
            updateModalSteps();
        }
    }

    document.getElementById('rental-form').addEventListener('submit', function (e) {
        e.preventDefault();
        handlePaymentSubmit();
    });
});

// Reset Modal State on Open
let rentalServices = []; // Store fetched services

window.openRentalModal = async function (vehicleId, dailyPrice, modelName, plateNo) {
    console.log("openRentalModal called with:", vehicleId, dailyPrice, modelName, plateNo);
    const token = localStorage.getItem('token');
    if (!token) {
        alert('Please login to rent a vehicle');
        window.location.href = 'login.html';
        return;
    }

    selectedVehicleId = vehicleId;
    currentDailyPrice = dailyPrice;

    // Reset Modal State
    // Reset Modal State
    currentStep = 1;
    // Explicitly show step 1
    const sDates = document.getElementById('step-dates');
    const sPay = document.getElementById('step-payment');
    const sPrice = document.getElementById('price-preview');
    if (sDates) sDates.style.display = 'block';
    if (sPay) sPay.style.display = 'none';
    if (sPrice) sPrice.style.display = 'none';

    document.getElementById('rental-form').reset();
    document.getElementById('modal-vehicle-info').innerHTML = `<h3 style="color:var(--primary-color)">${modelName}</h3><p style="color:var(--text-dim)">${plateNo}</p>`;
    document.getElementById('summary-daily-price').textContent = `$${dailyPrice}`;
    document.getElementById('summary-days').textContent = '0';
    document.getElementById('summary-total').textContent = '$0';

    const modal = document.getElementById('rental-modal');
    if (modal.parentNode !== document.body) {
        document.body.appendChild(modal);
        console.log("Moved modal to document.body");
    }

    modal.style.display = 'flex';
    modal.style.zIndex = '9999';
    modal.style.justifyContent = 'center';
    modal.style.alignItems = 'center';

    // Fix Inner Content Look
    const modalContent = modal.querySelector('.modal-content');
    if (modalContent) {
        modalContent.style.backgroundColor = '#1e293b'; // Solid dark background
        modalContent.style.padding = '2rem';
        modalContent.style.borderRadius = '15px';
        modalContent.style.boxShadow = '0 0 20px rgba(0,0,0,0.5)';
        modalContent.style.maxWidth = '500px';
        modalContent.style.width = '90%';
        modalContent.style.maxHeight = '90vh';
        modalContent.style.overflowY = 'auto';
    }

    // Fetch Additional Services
    const container = document.getElementById('additional-services-container');
    container.innerHTML = '<small>Loading services...</small>';
    try {
        const response = await fetch(`${API_URL}/AdditionalServices/getall`);
        const result = await response.json();
        rentalServices = result.data || result;

        container.innerHTML = '';
        if (rentalServices.length === 0) {
            container.innerHTML = '<small>No extra services available.</small>';
        } else {
            rentalServices.forEach(s => {
                const div = document.createElement('div');
                div.innerHTML = `
                    <label style="display:flex; align-items:center; gap:5px; font-weight:normal; cursor:pointer;">
                        <input type="checkbox" class="service-check" value="${s.serviceId}" data-cost="${s.cost}" onchange="calculateTotal()">
                        ${s.serviceName} (+$${s.cost})
                    </label>
                `;
                container.appendChild(div);
            });
        }
    } catch (e) {
        console.error(e);
        container.innerHTML = '<small style="color:red">Error loading services</small>';
    }
}

function calculateTotal() {
    const sDateVal = document.getElementById('start-date').value;
    const eDateVal = document.getElementById('end-date').value;

    const startDate = sDateVal ? new Date(sDateVal) : null;
    const endDate = eDateVal ? new Date(eDateVal) : null;

    // Validate dates
    if (startDate && endDate && endDate >= startDate) {
        const diffTime = Math.abs(endDate - startDate);
        // Include start day? Usually rental is per 24h block or calendar day. 
        // Let's assume min 1 day if same date, or diffDays.
        let diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
        if (diffDays === 0) diffDays = 1; // Minimum 1 day rental

        // Services Cost
        let servicesCost = 0;
        document.querySelectorAll('.service-check:checked').forEach(cb => {
            servicesCost += parseFloat(cb.dataset.cost);
        });

        const total = (diffDays * currentDailyPrice) + servicesCost;

        document.getElementById('summary-days').textContent = diffDays;
        document.getElementById('summary-services-cost').textContent = `$${servicesCost}`;
        document.getElementById('summary-total').textContent = `$${total}`;
        return total;
    } else {
        document.getElementById('summary-days').textContent = '0';
        document.getElementById('summary-services-cost').textContent = `$0`;
        document.getElementById('summary-total').textContent = '$0';
        return 0;
    }
}

async function handlePaymentSubmit() {
    // 1. Validate Payment Form (Fake)
    const cardName = document.getElementById('card-name').value;
    const cardNumber = document.getElementById('card-number').value;

    if (!cardName || !cardNumber) {
        alert('Please fill in payment details');
        return;
    }

    // 2. Prepare Reservation Object
    const startDate = document.getElementById('start-date').value;
    const endDate = document.getElementById('end-date').value;
    const totalAmount = parseFloat(document.getElementById('summary-total').textContent.replace('$', ''));

    // Get Selected Services
    const selectedServices = [];
    document.querySelectorAll('.service-check:checked').forEach(cb => {
        selectedServices.push({ serviceId: parseInt(cb.value) });
    });

    const reservation = {
        vehicleId: selectedVehicleId,
        customerId: 1, // Will be overridden by Backend using Token? Or we send from Token parse
        startDate: startDate,
        endDate: endDate,
        totalPrice: totalAmount,
        reservationServices: selectedServices // Send children
    };

    // Get CustomerId from Token
    const token = localStorage.getItem('token');
    try {
        const base64Url = token.split('.')[1];
        const payload = JSON.parse(window.atob(base64Url.replace(/-/g, '+').replace(/_/g, '/')));
        // Claim nameidentifier usually holds ID
        const userId = payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
        if (userId) reservation.customerId = parseInt(userId);
    } catch (e) {
        console.error("Token parse error", e);
    }

    // 3. Send API Request
    try {
        const response = await fetch(`${API_URL}/Reservations/add`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${token}`
            },
            body: JSON.stringify(reservation)
        });

        if (response.ok) {
            alert('Reservation Successful! Check your email.');
            document.getElementById('rental-modal').style.display = 'none';
            // Reset wizard
            currentStep = 1;
            updateModalSteps();
        } else {
            const err = await response.text();
            alert('Reservation Failed: ' + err);
        }
    } catch (error) {
        console.error('Error:', error);
        alert('An error occurred.');
    }
}


// Fetch Branches for Homepage
async function fetchBranches() {
    const listElement = document.getElementById('branch-list');
    if (!listElement) return;

    try {
        const response = await fetch(`${API_URL}/Branches/getall`);
        if (!response.ok) throw new Error('Failed to fetch branches');

        const result = await response.json();
        const branches = result.data ? result.data : result;

        listElement.innerHTML = '';

        if (branches.length === 0) {
            listElement.innerHTML = '<p style="color:var(--text-dim); text-align:center; grid-column:1/-1;">Our branches are expanding soon.</p>';
            return;
        }

        branches.forEach(branch => {
            const card = document.createElement('div');
            card.className = 'branch-card';
            card.innerHTML = `
                <div class="branch-map">
                    <span>Example Map Display</span>
                </div>
                <div class="branch-info">
                    <h3>${branch.branchName}</h3>
                    <p>📍 ${branch.city}</p>
                    <p>📞 Contact Support</p>
                </div>
            `;
            listElement.appendChild(card);
        });

    } catch (error) {
        console.error('Error fetching branches:', error);
        listElement.innerHTML = `<p style="color:red; text-align:center;">Could not load branch information.</p>`;
    }
}

// Initial Load
document.addEventListener('DOMContentLoaded', () => {
    fetchVehicles();
    fetchBranches();
});
