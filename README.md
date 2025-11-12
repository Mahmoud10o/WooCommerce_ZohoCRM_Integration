# WooCommerce to Zoho CRM Integration Service

A production-ready .NET 8 Worker Service that automatically synchronizes customer orders from WooCommerce into Zoho CRM as Contacts and Deals. This service demonstrates professional API integration patterns including OAuth2 authentication, REST API consumption, and background processing.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Setup](#api-setup)
- [Running the Service](#running-the-service)
- [How It Works](#how-it-works)
- [Testing](#testing)
- [Deployment](#deployment)
- [Project Structure](#project-structure)

---

## 🎯 Overview

This integration service solves a common business problem: automatically syncing e-commerce orders into your CRM system. Instead of manually entering customer data, this service:

1. **Monitors** your WooCommerce store for new orders (polls every 5 minutes)
2. **Extracts** customer and order information from WooCommerce REST API
3. **Creates/Updates** contacts in Zoho CRM automatically
4. **Creates** deals linked to those contacts with order details

**Why this approach over webhooks/Deluge?**
- ✅ Demonstrates deep understanding of REST APIs and OAuth2
- ✅ Full control over authentication, error handling, and retry logic
- ✅ Production-ready architecture with proper logging and service patterns
- ✅ Can be easily extended to support multiple stores or CRMs

---

## 🏗️ Architecture

```
┌───────────────────────────────────────────────────────────── ┐
│                    .NET Worker Service                       │
│                   (Background Service)                       │
├───────────────────────────────────────────────────────────── ┤
│                                                              │
│  ┌──────────────────┐         ┌────────────────────┐         │
│  │  Integration     │         │  Background Worker │         │
│  │  Worker          │────────▶│(Polls every 1.5 min)|       |
│  │  (Main Process)  │         └────────────────────┘         │
│  └──────────────────┘                                        │
│           │                                                  │
│           │                                                  │
│  ┌────────▼──────────┐         ┌────────────────────┐        │
│  │  WooCommerce      │         │  Zoho CRM Service   │       │
│  │  Service          │         │  (OAuth2 + API)     │       │
│  │  (Basic Auth)     │         └────────────────────┘        │
│  └───────────────────┘                    │                  │
│           │                               │                  │
│           │                               │                  │
│  ┌────────▼──────────┐         ┌─────────▼──────────┐        │
│  │  Integration      │         │   Data Models      │        │
│  │  Mapper           │         │   (DTOs)           │        │
│  └───────────────────┘         └────────────────────┘        │
│                                                              │
└─────────────────────────────────────────────────────────────┘
           │                                │
           │                                │
           ▼                                ▼
┌────────────────────┐         ┌────────────────────┐
│  WooCommerce       │         │   Zoho CRM         │
│  REST API          │         │   API v8           │
│  (Local Store)     │         │   (Cloud)          │
└────────────────────┘         └────────────────────┘
```

### Key Components:

- **IntegrationWorker**: Background service that runs continuously and polls for new orders
- **WooCommerceService**: Handles all communication with WooCommerce REST API
- **ZohoCRMService**: Manages OAuth2 authentication and Zoho CRM API calls
- **IntegrationMapper**: Converts WooCommerce order data to CRM contact/deal format

---

## ✨ Features

### Core Functionality
- ✅ **Automatic Order Synchronization** - Polls WooCommerce every 1.5 minutes for new orders
- ✅ **Contact Management** - Creates new contacts or updates existing ones (by email)
- ✅ **Deal Creation** - Automatically creates deals linked to contacts
- ✅ **OAuth2 Implementation** - Full OAuth2 flow with automatic token refresh
- ✅ **Duplicate Prevention** - Searches for existing contacts before creating new ones

### Technical Features
- ✅ **Clean Architecture** - Service interfaces, dependency injection, separation of concerns
- ✅ **Comprehensive Logging** - Built-in .NET logging for monitoring and debugging
- ✅ **Error Handling** - Graceful error handling with retry capability
- ✅ **Configuration Management** - Externalized config via appsettings.json

### API Integration Demonstrations
- ✅ **WooCommerce REST API** - Basic Authentication, pagination(100 order max for zoho crm), filtering(by date)
- ✅ **Zoho CRM API v8** - OAuth2, CRUD operations, search functionality
- ✅ **Token Management** - Automatic access token refresh before expiry

---

## 📦 Prerequisites

### Software Requirements

1. **.NET 9 SDK** (Free)
   - Download: https://dotnet.microsoft.com/download/dotnet/9.0
   - Why: This is the runtime and development kit for running .NET applications
   - Check if installed: `dotnet --version` (should show 9.x.x)

2. **LocalWP** (Free - for WooCommerce)
   - Download: https://localwp.com/
   - Why: Runs WordPress + WooCommerce locally without needing a web host
   - Alternative: Any WordPress installation with WooCommerce plugin

3. **Zoho CRM Account** (Free Trial)
   - Sign up: https://www.zoho.com/crm/signup.html
   - Why: The CRM where customer data will be stored
   - You'll need: Client ID, Client Secret, and Refresh Token (explained below)

---

## 🚀 Installation

### Step 1: Clone the Repository

```bash
git clone https://github.com/yourusername/woocommerce-zoho-integration.git
cd woocommerce-zoho-integration
```

### Step 2: Verify .NET Installation

```bash
dotnet --version
```

You should see something like `9.0.x`. If not, install .NET 9 SDK from the link above.

### Step 3: Restore Dependencies

```bash
dotnet restore
```

### Step 4: Build the Project

```bash
dotnet build
```

This compiles the C# code. You should see "Build succeeded" message.

---

## ⚙️ Configuration

### Understanding appsettings.json

This file contains all your API credentials and settings.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "WooCommerce": {
    "BaseUrl": "https://0da7683589c6.ngrok-free.app/wp-json/wc/v3",
    "ConsumerKey": "your_woocommerce_consumerkey",
    "ConsumerSecret": "your_woocommerce_consumersecret"
  },
  "ZohoCRM": {
    "ApiBaseUrl": "https://www.zohoapis.com/crm/v8",
    "AccountsUrl": "https://accounts.zoho.com/oauth/v2/token?",
    "ClientId": "zoho api console client id",
    "ClientSecret": "zoho api console client secret",
    "RefreshToken": "refresh token form zoho (to get access token)"
  }
}
```

### Configuration Breakdown:

| Setting | Description | Example |
|---------|-------------|---------|
| `WooCommerce:BaseUrl` | Your local WooCommerce API endpoint | `http://mystore.local/wp-json/wc/v3` |
| `WooCommerce:ConsumerKey` | API key from WooCommerce (starts with ck_) | `ck_abc123...` |
| `WooCommerce:ConsumerSecret` | API secret from WooCommerce (starts with cs_) | `cs_def456...` |
| `ZohoCRM:ClientId` | OAuth2 Client ID from Zoho (starts with 1000.) | `1000.ABC123XYZ` |
| `ZohoCRM:ClientSecret` | OAuth2 Client Secret from Zoho | `xxxxxxxx` |
| `ZohoCRM:RefreshToken` | Long-lived token for getting access tokens | `1000.xxxx.xxxx` |

---

## 🔑 API Setup

### Part A: WooCommerce API Credentials

#### Step 1: Access WooCommerce Settings

1. Open your local WordPress site (e.g., `http://mystore.local/wp-admin`)
2. Login with your admin credentials
3. Go to: **WooCommerce** → **Settings** → **Advanced** → **REST API**

#### Step 2: Generate API Keys

1. Click **"Add Key"** button
2. Fill in the form:
   - **Description**: `Integration Service` (or any name)
   - **User**: Select your admin user
   - **Permissions**: Select **Read/Write**
3. Click **"Generate API Key"**
4. **IMPORTANT**: Copy the Consumer Key and Consumer Secret immediately!
   - They look like:
     - Consumer Key: `ck_1234567890abcdef1234567890abcdef12345678`
     - Consumer Secret: `cs_1234567890abcdef1234567890abcdef12345678`
   - **You can only see these once**

#### Step 3: Test WooCommerce API

Using curl (command line):
```bash
curl -u "ck_YOUR_KEY:cs_YOUR_SECRET" http://yoursite.local/wp-json/wc/v3/orders
```

Using browser: Visit `http://yoursite.local/wp-json/wc/v3` to see available endpoints.

You should get a JSON response with order data (or empty array if no orders yet).

---

### Part B: Zoho CRM OAuth2 Setup

This is the trickiest part

#### Understanding OAuth2 Flow

OAuth2 is a secure way to access APIs without sharing passwords. Here's what you need:

1. **Client ID** - Your application's identifier (public)
2. **Client Secret** - Your application's password (private)
3. **Authorization Code** - One-time code to prove user consent (expires quickly(3 , 5 , 10) minutes probably)
4. **Refresh Token** - Long-lived token to get access tokens (this is what you store)
5. **Access Token** - Short-lived token for API calls (refreshed automatically by our service)

#### Step 1: Create Zoho self-client Application (or any other app according to your needs)

1. Go to: https://api-console.zoho.com/
2. Click **"Add Client"**
3. Choose **"self-client Applications"**
4. Fill in the form:
   - **Client Name**: `WooCommerce Integration` (any name)
   - **Description**: (any description)
5. Click **"Create"**
6. You'll see your **Client ID** and **Client Secret**
   - Copy these immediately!
   - Example:
     - Client ID: `1000.ABCDEFGHIJKLMNOP`
     - Client Secret: `1234567890abcdef1234567890abcdef`

#### Step 2: Generate Authorization Code

We need to get user permission to access their CRM. Build this URL (replace YOUR_CLIENT_ID):

```
https://accounts.zoho.com/oauth/v2/auth?scope=ZohoCRM.modules.ALL,ZohoCRM.settings.ALL&client_id=YOUR_CLIENT_ID&response_type=code&access_type=offline&redirect_uri=http://localhost:5000/callback
```

**Full example:**
```
https://accounts.zoho.com/oauth/v2/auth?scope=ZohoCRM.modules.ALL,ZohoCRM.settings.ALL&client_id=1000.ABCDEFGHIJKLMNOP&response_type=code&access_type=offline&redirect_uri=http://localhost:5000/callback
```

1. Paste this URL in your browser
2. Login to your Zoho account
3. Click **"Accept"** to authorize the application
4. You'll be redirected to: `http://localhost:5000/callback?code=1000.xxxxx.xxxxxx`
5. The page won't load (that's OK!), just copy the **code** from the URL
   - The code is the value after `code=`
   - Example: `1000.abc123def456ghi789jkl012mno345pqr678`
   - **This code expires in 10 minutes! Use it quickly in the next step.**

#### Step 3: Generate Refresh Token

Now we exchange the authorization code for a refresh token.

**Using curl:**
```bash
curl -X POST "https://accounts.zoho.com/oauth/v2/token" \
  -d "grant_type=authorization_code" \
  -d "client_id=YOUR_CLIENT_ID" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "redirect_uri=http://localhost:5000/callback" \
  -d "code=YOUR_AUTHORIZATION_CODE"
```

**Real example:**
```bash
curl -X POST "https://accounts.zoho.com/oauth/v2/token" \
  -d "grant_type=authorization_code" \
  -d "client_id=1000.ABCDEFGHIJKLMNOP" \
  -d "client_secret=1234567890abcdef1234567890abcdef" \
  -d "redirect_uri=http://localhost:5000/callback" \
  -d "code=1000.abc123def456ghi789jkl012mno345pqr678"
```

**Response (JSON):**
```json
{
  "access_token": "1000.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "refresh_token": "1000.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "expires_in": 3600,
  "token_type": "Bearer"
}
```

**Copy the `refresh_token` value** - this is what you'll put in `appsettings.json`.

#### Step 4: Update appsettings.json

Now update your configuration file with all the credentials:

```json
{
  "ZohoCRM": {
    "ApiBaseUrl": "https://www.zohoapis.com/crm/v8",
    "ClientId": "1000.ABCDEFGHIJKLMNOP",
    "ClientSecret": "1234567890abcdef1234567890abcdef",
    "RefreshToken": "1000.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
  }
}
```

#### Step 5: Test Zoho API Access

The service will automatically handle getting access tokens, but you can test manually:

```bash
# Get access token from refresh token
curl -X POST "https://accounts.zoho.com/oauth/v2/token?refresh_token=YOUR_REFRESH_TOKEN&client_id=YOUR_CLIENT_ID&client_secret=YOUR_CLIENT_SECRET&grant_type=refresh_token"

# Use access token to test CRM API
curl -H "Authorization: Zoho-oauthtoken YOUR_ACCESS_TOKEN" \
  https://www.zohoapis.com/crm/v8/Contacts
```

---

## Running the Service

### Development Mode (Recommended for Testing)

```bash
# Navigate to project folder
cd WooCommerceZohoIntegration

# Run the service
dotnet run
```

**What you'll see:**
```
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: IntegrationWorker[0]
      Integration Worker Service started at: 11/12/2025 10:00:00 AM
info: IntegrationWorker[0]
      Checking for new orders...
info: IntegrationWorker[0]
      No new orders found
```

The service will:
- Start up and log that it's running
- Continue checking every 1.5 minutes
- Log all activities to console

### Testing the Integration

1. **Keep the service running** in one terminal
2. **Place a test order** in your WooCommerce store:
   - Go to your local store (e.g., `http://mystore.local`)
   - Add products to cart
   - Complete checkout with test data
3. **Watch the logs** - within 1.5 minutes you should see:
   ```
   info: IntegrationWorker[0]
         Checking for new orders...
   info: IntegrationWorker[0]
         Found 1 new orders to process
   info: IntegrationWorker[0]
         Processing Order #123 for customer@example.com
   info: ZohoCRMService[0]
         Creating new contact for customer@example.com
   info: IntegrationWorker[0]
         Contact created/updated with ID: 123456789012345678
   info: IntegrationWorker[0]
         Deal created with ID: 987654321098765432
   info: IntegrationWorker[0]
         Successfully processed Order #123
   ```
4. **Verify in Zoho CRM**:
   - Login to Zoho CRM
   - Go to **Contacts** - you should see the new customer
   - Go to **Deals** - you should see a deal linked to that contact

### Manual Testing (Trigger Immediately)

If you don't want to wait 5 minutes, you can modify the polling interval:

In `appsettings.json`:
```json
"Integration": {
  "PollingIntervalMinutes": 0.5  // Check every 30 seconds
}
```

Or restart the service after placing an order.

---

## 📊 How It Works

### Workflow Diagram

```
┌──────────────────────────────────────────────────────────────┐
│ 1. Customer places order on WooCommerce store                │
│    (Local WordPress site)                                     │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │ Order stored in
                     │ WooCommerce database
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 2. Integration Worker wakes up (every 5 minutes)             │
│    - Calls: GET /wp-json/wc/v3/orders?after=TIMESTAMP        │
│    - Authentication: Basic Auth (Consumer Key:Secret)         │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │ Returns new orders
                     │ in JSON format
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 3. For each new order:                                        │
│    a) Extract customer info (name, email, address)           │
│    b) Extract order info (products, total, date)             │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 4. Map to CRM format                                          │
│    WooCommerce → Zoho CRM field mapping                       │
│    - billing.first_name → First_Name                          │
│    - billing.email → Email                                    │
│    - order.total → Deal Amount                                │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 5. Check if contact exists                                    │
│    - Call: GET /crm/v8/Contacts/search?email=xxx             │
│    - Authentication: OAuth2 Bearer Token                      │
│    - If exists: Update contact                                │
│    - If not: Create new contact                               │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │ Returns Contact ID
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 6. Create Deal linked to Contact                             │
│    - Call: POST /crm/v8/Deals                                │
│    - Include: Deal_Name, Amount, Stage, Contact_Name.id      │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     │ Success!
                     │
┌────────────────────▼─────────────────────────────────────────┐
│ 7. Log success and continue to next order                    │
│    - Contact and Deal now visible in Zoho CRM                │
└──────────────────────────────────────────────────────────────┘
```

### Code Flow Explained

1. **Program.cs** - Entry point
   - Sets up the service
   - Registers dependencies (like telling the app "when someone asks for IWooCommerceService, give them WooCommerceService")
   - Starts the worker

2. **IntegrationWorker** - The main loop
   - Runs in background continuously
   - Every 1.5 minutes calls `GetOrdersSinceAsync()`
   - For each order, calls `ProcessOrderAsync()`

3. **WooCommerceService** - WooCommerce API wrapper
   - Handles authentication (Basic Auth)
   - Makes HTTP GET requests
   - Deserializes JSON response to C# objects

4. **ZohoCRMService** - Zoho CRM API wrapper
   - Handles OAuth2 (getting and refreshing tokens)
   - Makes HTTP POST/PUT/GET requests
   - Creates/updates contacts and deals

5. **IntegrationMapper** - Data transformer
   - Converts WooCommerce order object → Zoho contact object
   - Converts WooCommerce order object → Zoho deal object

### Key Technical Concepts

#### Dependency Injection (DI)
```csharp
services.AddSingleton<IWooCommerceService, WooCommerceService>();
```
- This means: "Whenever code asks for IWooCommerceService, give it a WooCommerceService instance"
- Benefits: Easy testing, loose coupling, clean architecture

#### Background Service (HostedService)
```csharp
public class IntegrationWorker : BackgroundService
```
- Special .NET class that runs continuously in background
- Like a daemon/service in Linux or Windows Service
- Automatically restarted if it crashes

#### HttpClient with Basic Auth
```csharp
var authToken = Convert.ToBase64String(
    Encoding.ASCII.GetBytes($"{consumerKey}:{consumerSecret}"));
client.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Basic", authToken);
```
- WooCommerce uses Basic Auth: `Authorization: Basic base64(key:secret)`

#### OAuth2 Token Refresh
```csharp
if (DateTime.UtcNow >= _tokenExpiry) {
    // Refresh token before it expires
}
```
- Access tokens expire in 1 hour
- Service automatically refreshes them using the refresh token
- No manual intervention needed

---

### Integration Testing Checklist

- [ ] WooCommerce API credentials work (test with curl)
- [ ] Zoho OAuth2 flow complete (refresh token obtained)
- [ ] Service starts without errors
- [ ] Logs show "Checking for new orders..."
- [ ] Place test order in WooCommerce
- [ ] Service processes order (watch logs)
- [ ] Contact appears in Zoho CRM
- [ ] Deal appears in Zoho CRM linked to contact

---

##  Deployment

### Windows Service

```bash
# Publish the application
dotnet publish -c Release -o ./publish

# Install as Windows Service (requires admin)
sc create "WooCommerceZohoIntegration" binPath="C:\path\to\publish\WooCommerceZohoIntegration.exe"

# Start the service
sc start WooCommerceZohoIntegration

# Check status
sc query WooCommerceZohoIntegration

# View logs
# Check Windows Event Viewer or configure file logging
```

---

##  Project Structure

```
WooCommerceZohoIntegration/
│
├── Program.cs                          # Main entry point and service configuration
│   ├── Main()                          # App startup
│   ├── IntegrationWorker              # Background service class
│   ├── WooCommerceService             # WooCommerce API client
│   ├── ZohoCRMService                 # Zoho CRM API client with OAuth2
│   ├── IntegrationMapper              # Data transformation logic
│   └── Models/                         # Data transfer objects (DTOs)
│       ├── WooCommerceOrder           # Order model
│       ├── BillingDetails             # Customer billing info
│       ├── LineItem                   # Order line
