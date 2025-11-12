
# WooCommerce to Zoho CRM Integration Service

A production-ready .NET 9 Worker Service that automatically synchronizes customer orders from WooCommerce into Zoho CRM as Contacts and Deals. This service demonstrates professional API integration patterns including OAuth2 authentication, REST API consumption, and background processing.

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

1. **Monitors** your WooCommerce store for new orders (polls every 1.5 minutes)
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
│  │  Worker          │────────▶│(Polls every 1.5 min)|       │
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
- ✅ **OAuth2 Implementation** - Full OAuth2 flow with automatic token refresh (self-client guidance below)
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

> (No changes here — follow the same steps in your existing README to generate Woocommerce consumer keys and test the API.)

---

### Part B: Zoho CRM OAuth2 Setup (Self-Client — recommended for testing and single-account integrations)

> This section replaces the general OAuth2 flow with a clearer **self-client** flow which is easier to use for development, one-off integrations, or when you are the only Zoho account owner. The _self-client_ approach lets you generate an authorization code directly from Zoho's API Console and exchange it for a refresh token — you don't need to host a redirect/callback endpoint for this manual setup step.

#### Why use a Self-Client?
- Faster for development and testing.
- No redirect/callback server required when generating the initial authorization code.
- Produces the same long-lived **refresh token** that the integration service uses to obtain short-lived access tokens.

> **Important**: Self-client is ideal for development or single-account automation. For multi-user or public applications, implement the standard OAuth2 authorization code flow with a redirect URI and consent screen.

#### Step 1: Create a Self-Client in Zoho API Console

1. Open the Zoho API Console: `https://api-console.zoho.com/`
2. Click **Add Client** → choose **Self-Client** (or similar option in the console)
3. Provide a Client Name and Description and create the client
4. Copy your **Client ID** and **Client Secret** — store them securely

#### Step 2: Generate Authorization Code (Grant Token) using the Console

- In the API Console, find your Self-Client and use the **Generate Code** feature.
- Choose the scopes your service needs. For this integration the recommended scopes are:
  - `ZohoCRM.modules.ALL`  (full access to CRM modules the account has permission to use)
  - `ZohoCRM.settings.ALL` (if you need settings-level access)

- Click **Generate** (or **Get Code**). The console will produce an **authorization code** (sometimes called a grant token). Copy it immediately — it is short lived.

> **Alternative (browser URL)**: If you prefer the manual URL approach, you can also open a browser with an authorization URL (replace region/domain and client id):

> ```
> https://accounts.zoho.com/oauth/v2/auth?scope=ZohoCRM.modules.ALL,ZohoCRM.settings.ALL&client_id=YOUR_CLIENT_ID&response_type=code&access_type=offline
> ```

> After login+consent the URL will contain `?code=...` which you copy. But for self-client users it's simpler to use the API Console's Generate Code UI.

#### Step 3: Exchange Authorization Code for Refresh Token

Use the token endpoint to exchange the authorization code for an access token and a **refresh token**.

**Token endpoint (region-aware)**
- US / global: `https://accounts.zoho.com/oauth/v2/token`
- EU: `https://accounts.zoho.eu/oauth/v2/token`
- India: `https://accounts.zoho.in/oauth/v2/token`
- China: `https://accounts.zoho.com.cn/oauth/v2/token`

**cURL example (replace values):**

```bash
curl -X POST "https://accounts.zoho.com/oauth/v2/token" \
  -d "grant_type=authorization_code" \
  -d "client_id=YOUR_CLIENT_ID" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "redirect_uri=http://localhost" \
  -d "code=YOUR_AUTHORIZATION_CODE"
```

**Expected JSON response:**

```json
{
  "access_token": "1000.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "refresh_token": "1000.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx.xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "expires_in": 3600,
  "token_type": "Bearer"
}
```

- **Copy the `refresh_token`** to your `appsettings.json` under `ZohoCRM:RefreshToken` — this is the token your service will use to obtain access tokens programmatically.

#### Step 4: Refreshing Access Tokens (what the service does automatically)

The integration service uses the refresh token to obtain short-lived access tokens. Example curl to refresh:

```bash
curl -X POST "https://accounts.zoho.com/oauth/v2/token" \
  -d "refresh_token=YOUR_REFRESH_TOKEN" \
  -d "client_id=YOUR_CLIENT_ID" \
  -d "client_secret=YOUR_CLIENT_SECRET" \
  -d "grant_type=refresh_token"
```

Response will include a new `access_token` valid for ~1 hour. The service refreshes tokens automatically and caches the current access token until near expiry.

#### Region / Domain Notes
- Use the accounts endpoint that matches your Zoho data center (examples above). The CRM API base URL also varies by region: `https://www.zohoapis.com` (US/global), `https://www.zohoapis.eu` (EU), `https://www.zohoapis.in` (India), `https://www.zohoapis.com.cn` (China).
- Mixing endpoints from different regions will fail — ensure `AccountsUrl` and `ApiBaseUrl` match the same Zoho region for your account.

#### Security Notes
- Treat `ClientSecret` and `RefreshToken` as sensitive secrets — don't commit them to git.
- Store secrets in environment variables or a secrets store for production deployments.

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

If you don't want to wait 1.5 minutes, you can modify the polling interval:

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
- [ ] Zoho OAuth2 flow complete (refresh token obtained using Self-Client)
- [ ] Service starts without errors
- [ ] Logs show "Checking for new orders..."
- [ ] Place test order in WooCommerce
- [ ] Service processes order (watch logs)
- [ ] Contact appears in Zoho CRM
- [ ] Deal appears in Zoho CRM linked to contact

---

## Deployment

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

## Project Structure

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
```
