# JWT Example code

This is example code for [A secure implementation of JSON Web Tokens (JWT) in C#](https://medium.com/swlh/a-secure-implementation-of-json-web-tokens-jwt-in-c-710d06ea243).

Warning: This is very basic and doesn't do things like hashing passwords! Only to test JSON Web Tokens.

To test this, you need to have Dotnet Core 3 and MongoDB installed. Settings are in appsettings.json.

This uses the following packages:

-   [MongoDB.Driver](https://www.nuget.org/packages/MongoDB.Driver/2.10.3)
-   [Microsoft.AspNetCore.Authentication.JwtBearer](https://www.nuget.org/packages/Microsoft.AspNetCore.Authentication.JwtBearer)
-   [JWT](https://www.nuget.org/packages/JWT)

After restoring the packages and starting it, do the following to test the JWT:

1. POST https://localhost:5001/users with the following body:

```javascript
{
    "username": "yourusername",
    "password": "yourpassword"
}
```

2. POST https://localhost:5001/tokens/accesstoken with the same body. You get the following response:

```javascript
{
    "accessToken": "*jwt*",
    "refreshToken": "*jwt*"
}
```

3. To test the access token do a GET request to https://localhost:5001/users. In the Authorization header of the request should be "Bearer " and then the access token you got in step 2. It will give you a response with the user you created in step 1.

4. To test the refresh token do a PUT request to https://localhost:5001/tokens/accesstoken. In the Authorization header of the request should be "Bearer " and then the refresh token you got in step 2. It should respond with a new access token and a new refresh token. Try again to test if the refresh token is deleted, which it should.
