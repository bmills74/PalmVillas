This is a ASP.NET Core web app with Razor Pages.
It's purpose is to allow users to book a holiday home in a villa complex in Bali.
It features:
- Google sign-on and authentication cookie authorization
- Sqlite db and EFCore with Code First
- Db logging using Nlog
- Role based authorization using Microsoft identity
- Database service layer incorporated using dependency injection
- Unit and integration tests implemented using Nunit and Moq. (Not completely full coverage yet)
- Bootstrap css used for a lot of the ui

  **Please note**. This is NOT intended as a production site, it has been developed purely as a personal project

  **If running locally:** You need to ensure your ssl port is 44358 under properties/launchsettings.json in order for google sign-on to work
