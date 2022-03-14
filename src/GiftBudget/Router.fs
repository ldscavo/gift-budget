module Router

open Saturn
open Giraffe.Core
open Microsoft.AspNetCore.Authentication.Cookies

let browser =
    pipeline {
        plug acceptHtml
        plug putSecureBrowserHeaders
        plug fetchSession
        set_header "x-pipeline-type" "Browser"
    }

let requires_login =
    pipeline {
        requires_authentication (Giraffe.Auth.challenge CookieAuthenticationDefaults.AuthenticationScheme)    
    }

let defaultView =
    router {
        get "/" (htmlView Index.layout)
        get "/index.html" (redirectTo false "/")
        get "/default.html" (redirectTo false "/")

        // routes that do not require being logged in
        forward "/login" Users.Controller.login
    }

let loggedInRouter =
    router {
        pipe_through requires_login
        forward "/logout" Users.Controller.logout
        forward "/test" Users.Controller.loggedInTest
    }

let browserRouter =
    router {
        not_found_handler (htmlView NotFound.layout) //Use the default 404 webpage
        pipe_through browser //Use the default browser pipeline
    
        forward "" defaultView //Use the default view    
        forward "" loggedInRouter
    }

let appRouter =
    router {
        forward "" browserRouter
    }