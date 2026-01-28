function parseJwt(token) {
    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    } catch (e) {
        return null;
    }
}

let currentUserId = 0;

document.addEventListener('DOMContentLoaded', async () => {
    const token = localStorage.getItem('token');
    if (!token) {
        window.location.href = 'index.html'; // Redirect if not logged in
        return;
    }

    // Extract User ID from Token
    // Claim Types usually: 
    // "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": "ID" (String)
    const payload = parseJwt(token);
    if (payload) {
        // Look for claim ending in nameidentifier or just use standard keys if custom
        const idClaim = Object.keys(payload).find(key => key.endsWith('nameidentifier'));
        if (idClaim) {
            currentUserId = parseInt(payload[idClaim]);
        }
    }

    if (!currentUserId) {
        alert("Could not identify user. Please login again.");
        window.location.href = 'login.html';
        return;
    }

    // Set Avatar Initial
    const email = localStorage.getItem('userEmail') || "U";
    document.getElementById('avatarInitial').innerText = email.charAt(0).toUpperCase();

    // Load Profile
    await loadProfile(currentUserId);

    // Handle Submit
    document.getElementById('profileForm').addEventListener('submit', handleProfileUpdate);
});

async function loadProfile(id) {
    try {
        const response = await fetch(`${API_URL}/Customers/getbyid?id=${id}`, {
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });

        if (response.status === 401) {
            alert('Session expired');
            window.location.href = 'login.html';
            return;
        }

        const result = await response.json();
        const user = result.data || result;

        if (user) {
            document.getElementById('firstName').value = user.firstName;
            document.getElementById('lastName').value = user.lastName;
            document.getElementById('email').value = user.email;
            document.getElementById('phone').value = user.phone || "";
            document.getElementById('licenseNo').value = user.licenseNo || "";
            document.getElementById('address').value = user.address || "";
        }
    } catch (error) {
        console.error('Error loading profile:', error);
        alert('Failed to load profile.');
    }
}

async function handleProfileUpdate(e) {
    e.preventDefault();

    const updatedUser = {
        id: currentUserId,
        firstName: document.getElementById('firstName').value,
        lastName: document.getElementById('lastName').value,
        email: document.getElementById('email').value,
        phone: document.getElementById('phone').value,
        licenseNo: document.getElementById('licenseNo').value,
        address: document.getElementById('address').value,
        status: true,
        // Send dummy values to satisfy [Required] validation
        // Backend Manager will overwrite these with existing DB values
        passwordHash: "ZHVtbXk=", // "dummy" in base64
        passwordSalt: "ZHVtbXk="
    };

    const btn = e.target.querySelector('button');
    const originalText = btn.innerText;
    btn.disabled = true;
    btn.innerText = 'Saving...';

    try {
        const response = await fetch(`${API_URL}/Customers/update`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('token')}`
            },
            body: JSON.stringify(updatedUser)
        });

        if (response.ok) {
            alert('Profile updated successfully!');
            // Update local storage email if changed (though email is readonly in UI usually)
            localStorage.setItem('userEmail', updatedUser.email);
        } else {
            const err = await response.text();
            alert('Update failed: ' + err);
        }
    } catch (error) {
        console.error(error);
        alert('Error saving profile');
    } finally {
        btn.disabled = false;
        btn.innerText = originalText;
    }
}
