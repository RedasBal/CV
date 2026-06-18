# Deploying to Azure App Service (Free F1)

This publishes the CV site to a public URL like
`https://redas-cv.azurewebsites.net` — always on, no need to run it locally.

## Prerequisites (one-time)

1. **A free Azure account** — https://azure.microsoft.com/free
   (Microsoft account/email + card for identity check; the **F1 tier used here is free**.)
2. **Azure CLI** — being installed now (`winget install Microsoft.AzureCLI`).

## Deploy in 3 commands

Open a **new** PowerShell window (so it picks up the freshly installed `az`),
then from the project folder:

```powershell
cd C:\Users\redux\RiderProjects\CV

# 1. Sign in (opens a browser — pick your Azure account)
az login

# 2. Deploy. Pick a globally-unique name (lowercase, letters/numbers/hyphens).
az webapp up --name redas-cv --sku F1 --runtime "DOTNET:8.0" --location westeurope --os-type Linux
```

`az webapp up` will:
- create a resource group + Free (F1) App Service plan,
- build & publish the app,
- upload it and start it.

When it finishes it prints the URL, e.g. `http://redas-cv.azurewebsites.net`
(HTTPS works too: `https://redas-cv.azurewebsites.net`).

> If `redas-cv` is taken, choose another name — it must be unique across Azure.

## Redeploying after you change the site

Just run the same `az webapp up` command again (it remembers settings via
`.azure/config`). Or set up GitHub auto-deploy (see below).

## Alternative: deploy from Rider (GUI, no CLI)

1. Install the **Azure Toolkit for Rider** plugin (Settings → Plugins).
2. Sign in to Azure in Rider.
3. Right-click the project → **Publish → Azure → Web App** → create a new
   Free (F1) app → Deploy.

## Notes

- The contact form writes to `App_Data/messages.jsonl`. On the Free tier this
  storage resets on app restart — fine for a demo. For durable messages, switch
  `Services/ContactStore.cs` to send an email or write to a database.
- Free F1 apps may cold-start slowly after long idle periods.
- Custom domain (e.g. `redas.dev`): add it under the App Service → Custom domains.
