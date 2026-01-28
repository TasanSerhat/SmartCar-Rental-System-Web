// API_URL is defined in api.js

const loginForm = document.getElementById('loginForm');
if (loginForm) {
    loginForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const btn = e.target.querySelector('button');
        const originalText = btn.innerText;

        btn.disabled = true;
        btn.innerText = 'Signing In...';

        try {
            const response = await fetch(`${API_URL}/Auth/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            const result = await response.json();

            if (response.ok && result.success) {
                // Save Token
                localStorage.setItem('token', result.token);
                localStorage.setItem('expiration', result.expiration);
                localStorage.setItem('userEmail', email);

                // Redirect
                if (email.includes('admin')) {
                    window.location.href = 'admin.html';
                } else {
                    window.location.href = 'index.html';
                }
            } else {
                alert('Login Failed: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            console.error(error);
            alert('Connection Error');
        } finally {
            btn.disabled = false;
            btn.innerText = originalText;
        }
    });
}

// Register Form Logic
const registerForm = document.getElementById('registerForm');
if (registerForm) {
    registerForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const firstName = document.getElementById('firstName').value;
        const lastName = document.getElementById('lastName').value;
        const email = document.getElementById('email').value;
        const password = document.getElementById('password').value;
        const phone = document.getElementById('phone').value;
        const licenseNo = document.getElementById('licenseNo').value;
        const address = document.getElementById('address').value;

        const btn = e.target.querySelector('button');
        const originalText = btn.innerText;

        btn.disabled = true;
        btn.innerText = 'Creating Account...';

        try {
            const response = await fetch(`${API_URL}/Auth/register`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    firstName,
                    lastName,
                    email,
                    password,
                    phone,
                    licenseNo,
                    address
                })
            });

            const result = await response.json();

            if (response.ok && result.success) {
                // Auto Login on Register Success
                localStorage.setItem('token', result.token);
                localStorage.setItem('expiration', result.expiration);
                localStorage.setItem('userEmail', email);

                alert('Registration Successful!');
                window.location.href = 'index.html';
            } else {
                alert('Registration Failed: ' + (result.message || 'Unknown error'));
            }
        } catch (error) {
            console.error(error);
            alert('Connection Error');
        } finally {
            btn.disabled = false;
            btn.innerText = originalText;
        }
    });
}
