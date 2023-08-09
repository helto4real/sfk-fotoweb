// Refresh token providing a url to api using fetch with credentials
async function refreshToken(url) {
    await fetch(url, {
        method: 'GET',
        credentials: 'include',
        redirect: 'follow'
    });
}
