# HTTPS Development Certificate Setup

## Problem
When you run the application with HTTPS, your browser shows security warnings because it doesn't trust the localhost certificate.

## Solution: Trust the Development Certificate

### Step 1: Export Certificate from Dev Container
Run this in the VS Code terminal:
```bash
dotnet dev-certs https --export-path tmp/localhost.crt --format PEM
```

### Step 2: Copy Certificate to Your Host Machine
In VS Code, drag the `localhost.crt` file from the `tmp` folder to your local machine.

### Step 3: Trust the Certificate on Your Host OS

**macOS:**
```bash
# Add to system keychain (requires admin password)
sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain localhost.crt

# Verify the certificate was added and is trusted
security find-certificate -c "localhost" -p /Library/Keychains/System.keychain
# If successful, you'll see the certificate in PEM format
```

**Windows:**
1. Double-click the `localhost.crt` file
2. Click "Install Certificate"
3. Choose "Local Machine" → "Place all certificates in the following store"
4. Click "Browse" → Select "Trusted Root Certification Authorities"
5. Click "Next" → "Finish"

**Linux:**
```bash
sudo cp localhost.crt /usr/local/share/ca-certificates/
sudo update-ca-certificates
```

### Step 4: Restart Your Browser
Close and reopen your browser, then visit the HTTPS URL.

## Alternative: Use HTTP for Development
If HTTPS setup is problematic, you can use the HTTP URLs instead:
- Dashboard: http://localhost:15136
- Web App: http://localhost:5234
- API: http://localhost:5586