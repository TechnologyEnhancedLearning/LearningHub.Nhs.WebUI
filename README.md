# Introduction 
This is the official open-source repository for the [Learning Hub](https://learninghub.nhs.uk/) platform. 

The Learning Hub is the national digital learning platform providing easy access to a wide range of educational resources and support for the health and care workforce and educators.

The Learning Hub is provided and supported by the Technology Enhanced Learning Platforms team at [NHS England](https://www.england.nhs.uk/).

# Getting Started

## Required installs
- [Visual Studio Professional 2022](https://visualstudio.microsoft.com/downloads/) or other suitable An IDE that supports the Microsoft Tech Stack
  - Make sure you have the [NPM Task Runner](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) extension
- SQL Server 2019
- [SQL Server Management Studio 18](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver15)
- [Git](https://git-scm.com/download)
- [Node](https://nodejs.org/en/download/) install specific/required version using NVM - see below.
- [SASS](https://www.sass-lang.com/install) for the command line
    - Specifically, follow the "Install Anywhere (Standalone)" guide. Simply download and extract the files somewhere, and point PATH at the dart-sass folder. This should allow you to use the "sass" command.
    - You don't want to install it via Yarn, as those are JavaScript versions that perform significantly worse.
- [.Net 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Azure storage emulator](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- [Azure storage explorer](https://azure.microsoft.com/en-gb/features/storage-explorer/#overview)
- [Node version manager (nvm)](https://github.com/coreybutler/nvm-windows/releases) - use this to install and use Node version 16.13.0 and NPM version 8.1.0 to work with this repository.

## Project dependencies

To run the code, you will also need to clone and build the [Learning Hub User API repository](https://github.com/TechnologyEnhancedLearning/LearningHub.Nhs.UserApi)

You will need a development copy of the Learning Hub SQL database - to obtain this, please contact support (see Contribute section below)

## Getting the code

Clone the repository from [GitHub](https://github.com/TechnologyEnhancedLearning/LearningHub.Nhs.WebUI):

```bash
git clone git@github.com:TechnologyEnhancedLearning/LearningHub.Nhs.WebUI.git
```

You should now be able to open the solution in your IDE by finding and opening the `LearningHub.Nhs.WebUI.sln` file.

# Build the solution

## Prerequisites
These instructions assume that you have already followed the Build and Test instructions for the [Learning Hub User API repository](https://github.com/TechnologyEnhancedLearning/LearningHub.Nhs.UserApi). Specifically, you have already:
- Created a Truested self-signed certificate
- Configured Learning Hub development URLs to use this certificate in IIS
- Added the development URLs to the Windows hosts files
- Set up the Learning Hub and elfh Hub databases and run necessary migrations
- Created a user account for login
- Added the NuGet package source to Visual Studio
- Built and run the User API application

## Compile Sass and TypeScript files using Yarn
1. Right click LearningHub.Nhs.WebUI/LearningHub.Nhs.WebUI in the solution explorer and choose 'Open in Terminal'
2. Run `npm clean-install`
3. Run `npm cache clean --force`
4. Run `npm install`
5. Run `npm run dev` (after the first successful build, this will be the only command required)

Or, for better performance, use [Yarn](https://classic.yarnpkg.com/lang/en/docs/install/):

1. Run `yarn install`
2. Run `yarn run watch`

**NOTE:** If you run into any errors, using a different version of node may help. It is best to use nvm - Node Version Manager - to do this.

Repeat the above for the LearningHub.Nhs.AdminUI project.

## Configure App Settings

Add appsettings.Development.json files to the following projects:
- LearningHub.Nhs.WebUI
- LearningHub.Nhs.Api
- LearningHub.Nhs.AdminUI
Modify settings as appropriate for your environment.

If you are an official contributor (see below) working appsettings.Development.json will be provided by the service team.

## Configure Local IIS profile

Create a Local IIS launch profile for the following projects:
- LearningHub.Nhs.WebUI
- LearningHub.Nhs.Api
- LearningHub.Nhs.AdminUI

1. From the launch drop down choose **Debug Properties**
2. Create a **New** profile
3. Choose **IIS** from the drop-down and name the profile **IIS Local**
4. Add the environment name to **Environment variables**: `ASPNETCORE_ENVIRONMENT=Development`
5. Check **Launch browser** if appropriate (WebUI, AdminUI)
6. Set the **App URL**. Suggested URLs are as follows:
   - LearningHub.Nhs.AdminUI = https://lh-admin.dev.local
   - LearningHub.Nhs.WebAPI = https://lh-api.dev.local
   - LearningHub.Nhs.WebUI = https://lh-web.dev.local
7. Tick the checkbox for **Enable Anonymous Authentication**. 

## Rebuild and run

1. Rebuild the solution
2. Set **LearningHub.Nhs.WebUI** as the startup Project
3. Start debugging.


# Contribute
If you are interested in contributing to the Learning Hub, please contact [support@learninghub.nhs.uk](mailto:support@learninghub.nhs.uk)
