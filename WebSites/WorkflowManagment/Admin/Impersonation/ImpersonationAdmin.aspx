<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>Super Admin Impersonation</title>
    <style type="text/css">
        body { font-family: Arial, sans-serif; margin: 20px; }
        .impersonation-container { max-width: 800px; margin: 0 auto; }
        .impersonation-form { background: #f5f5f5; padding: 20px; border-radius: 5px; }
        .user-list { margin: 20px 0; }
        .user-item { padding: 10px; border-bottom: 1px solid #ddd; }
        .user-item:hover { background: #f9f9f9; }
        .impersonate-btn { background: #d9534f; color: white; border: none; padding: 5px 15px; border-radius: 3px; cursor: pointer; }
        .impersonate-btn:hover { background: #c9302c; }
        .stop-impersonation { background: #5bc0de; color: white; border: none; padding: 5px 15px; border-radius: 3px; cursor: pointer; margin-left: 10px; }
        .stop-impersonation:hover { background: #31b0d5; }
        .status { padding: 10px; margin: 10px 0; border-radius: 3px; }
        .impersonating { background: #fcf8e3; border: 1px solid #faebcc; }
        .not-impersonating { background: #dff0d8; border: 1px solid #d6e9c6; }
        .current-user { font-weight: bold; color: #337ab7; }
        .impersonated-user { font-weight: bold; color: #d9534f; }
        .loading { display: none; }
    </style>
    <script type="text/javascript">
        function loadImpersonatableUsers() {
            // Show loading indicator
            document.getElementById('loading').style.display = 'block';
            
            // Make AJAX call to get impersonatable users
            var xhr = new XMLHttpRequest();
            xhr.open('GET', 'ImpersonationAdmin.aspx/GetImpersonatableUsers', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    document.getElementById('loading').style.display = 'none';
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        if (response.success) {
                            displayUsers(response.users);
                        } else {
                            alert('Error: ' + response.message);
                        }
                    } else {
                        alert('Error loading users');
                    }
                }
            };
            xhr.send();
        }
        
        function displayUsers(users) {
            var userListDiv = document.getElementById('userList');
            userListDiv.innerHTML = '';
            
            if (users.length === 0) {
                userListDiv.innerHTML = '<div class="status not-impersonating">No users available for impersonation.</div>';
                return;
            }
            
            for (var i = 0; i < users.length; i++) {
                var user = users[i];
                var userItem = document.createElement('div');
                userItem.className = 'user-item';
                userItem.innerHTML = 
                    '<span>' + user.firstName + ' ' + user.lastName + ' (' + user.userName + ') - ' + user.email + '</span>' +
                    '<button class="impersonate-btn" onclick="impersonateUser(' + user.id + ', \'' + user.userName + '\')">Impersonate</button>'
                    ;
                userListDiv.appendChild(userItem);
            }
        }
        
        function impersonateUser(userId, userName) {
            if (!confirm('Are you sure you want to impersonate ' + userName + '? You will be able to see their view and perform actions as them.')) {
                return;
            }
            
            // Show loading indicator
            document.getElementById('loading').style.display = 'block';
            
            // Make AJAX call to start impersonation
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'ImpersonationAdmin.aspx/StartImpersonation', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    document.getElementById('loading').style.display = 'none';
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        if (response.success) {
                            alert('Impersonation started successfully!');
                            loadImpersonationStatus();
                            loadImpersonatableUsers();
                        } else {
                            alert('Error: ' + response.message);
                        }
                    } else {
                        alert('Error starting impersonation');
                    }
                }
            };
            var data = JSON.stringify({ userId: userId });
            xhr.send(data);
        }
        
        function stopImpersonation() {
            if (!confirm('Are you sure you want to stop impersonation and return to your own account?')) {
                return;
            }
            
            // Show loading indicator
            document.getElementById('loading').style.display = 'block';
            
            // Make AJAX call to stop impersonation
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'ImpersonationAdmin.aspx/StopImpersonation', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    document.getElementById('loading').style.display = 'none';
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        if (response.success) {
                            alert('Impersonation stopped successfully!');
                            loadImpersonationStatus();
                            loadImpersonatableUsers();
                        } else {
                            alert('Error: ' + response.message);
                        }
                    } else {
                        alert('Error stopping impersonation');
                    }
                }
            };
            xhr.send();
        }
        
        function loadImpersonationStatus() {
            // Make AJAX call to get impersonation status
            var xhr = new XMLHttpRequest();
            xhr.open('GET', 'ImpersonationAdmin.aspx/GetImpersonationStatus', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.onreadystatechange = function() {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        if (response.success) {
                            displayImpersonationStatus(response);
                        }
                    }
                }
            };
            xhr.send();
        }
        
        function displayImpersonationStatus(data) {
            var statusDiv = document.getElementById('impersonationStatus');
            var currentUserSpan = document.getElementById('currentUser');
            var impersonatedUserSpan = document.getElementById('impersonatedUser');
            var stopBtn = document.getElementById('stopImpersonationBtn');
            
            currentUserSpan.textContent = data.currentUserName;
            
            if (data.isImpersonating) {
                statusDiv.className = 'status impersonating';
                statusDiv.innerHTML = '<strong>Impersonating:</strong> You are currently impersonating user ID ' + data.impersonatedUserId + '. ';
                stopBtn.style.display = 'inline-block';
            } else {
                statusDiv.className = 'status not-impersonating';
                statusDiv.innerHTML = '<strong>Status:</strong> You are not impersonating any user. ';
                stopBtn.style.display = 'none';
            }
            
            if (data.isImpersonating) {
                impersonatedUserSpan.style.display = 'inline';
                impersonatedUserSpan.textContent = '(Impersonated User: ' + data.impersonatedUserId + ')';
            } else {
                impersonatedUserSpan.style.display = 'none';
            }
        }
        
        // Load data when page loads
        window.onload = function() {
            loadImpersonationStatus();
            loadImpersonatableUsers();
        };
    </script>
</head>
<body>
    <div class="impersonation-container">
        <h1>Super Admin Impersonation Control Panel</h1>
        
        <div id="impersonationStatus" class="status">
            <strong>Status:</strong> Loading...
            <span id="impersonatedUser" style="display: none;"></span>
        </div>
        
        <div class="impersonation-form">
            <h2>Current User: <span id="currentUser" class="current-user">Loading...</span></h2>
            
            <div id="loading" class="loading">
                Loading...
            </div>
            
            <h3>Available Users for Impersonation</h3>
            <div id="userList" class="user-list">
                Loading users...
            </div>
            
            <button id="stopImpersonationBtn" class="stop-impersonation" onclick="stopImpersonation()" style="display: none;">
                Stop Impersonation
            </button>
        </div>
    </div>
</body>
</html>