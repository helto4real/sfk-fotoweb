// Refresh token providing a url to api using fetch with credentials
async function refreshToken(url) {
    await fetch(url, {
        method: 'GET',
        credentials: 'include',
        redirect: 'follow'
    });
}

// Set darkmode using cookies
async function setDarkMode(isDarkmode) {
    setCookie("UseDarkmode", isDarkmode);
}

function setCookie(name, value) {
    document.cookie = name + "=" + (value || "")  + "; expires=Fri, 31 Dec 9999 23:59:59 GMT; path=/";
}

async function login(url, data) {
    try {
        var response = await fetch(url, {
            method: 'POST',
            redirect: 'manual',
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(data)
        });
        if (response.ok) {
            var okResult = await response.json()
            return [okResult, null];
        } else {
            var errorResult = await response.json();
            return [null, errorResult];
        }
    } catch (error)
    {
        return [null, 
            {
                title: "Server error",
                statusCode: 500,
                detail: error
            }];
    }
}