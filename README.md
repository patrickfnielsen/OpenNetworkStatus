# OpenNetworkStatus
Open Source, API driven status page
![Image of OpenNetworkStatus](https://github.com/patrickfnielsen/OpenNetworkStatus/blob/master/screenshots/screenshot.png)

## Testing
Run the docker-compose file, using "docker-compose up"

It will run a Postgresql database, and an instance of OpenNetworkStatus on *localhost:5001*

## General
On every new install, a user will be created with the username ***admin*** and password ***password***, the first thing you should do is to change this password to something else.
See the API section for how to change the password.

All POST/PUT requests - and GET requests to the authentication endpoint - should be authorized with a JWT token in the authorization header.

# API
Basic API usage. Swagger support will be added in the next release.

## Authentication: Login
Login and retrive the JWT token used for subsequent requests to authenticate.
### Request
`POST /api/v1/authentication/login`

    {
        "username: "",
        "password: ""
    }

### Response
    {
        "token: "<token>",
        "expires_at: "2020-01-01T00:00:00.0000000Z"
    }
