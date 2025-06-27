# MagicDeckStats

An application for analyzing _Magic: The Gathering_ deck statistics and gameplay data, importing data from [BGStats](https://www.bgstatsapp.com/). This is a Blazor WebAssembly (WASM) application built with .NET 8.

I mainly use it to track performance of Battle Decks and see how balanced they are against each other.

## FAQ

### **How do I import data to this app?**
You need to export data first from BGStats:

1. Open BGStats on your device
2. Go to **"Settings"**
3. Select **"Export, import and backup"**
4. Select **"Export backup file..."**
5. Choose a location to save the file

This will give you a file called `BGStatsExport.json` that you can upload to the site.

### **What are Battle Decks?**

Battle Decks are, in my opinion, the by far best way to play casual constructed Magic: The Gathering.

I plan to write a long guide on how to get started in this wonderful "format", but briefly explained, they are very affordable pre-constructed decks that Card Kingdom provides (they build and sell them), mostly made up of cheap and otherwise unwanted cards. They are balanced to have approximately the same power level, so they can be mixed and matched against each other without many problems. In fact, this app was created just to demonstrate how well-balanced they are. Any meta in Magic: The Gathering will have decks with higher or lower win rates - that's normal and expected, so with that in mind, it's amazing how well they have balanced these decks.

### **Why is the code shit?**
I have "vibe coded" this whole app because I'm disgusting.

### **Is my data stored on a server?**
No, this is a client-side application. All data processing happens in your browser, and no data is sent to external servers. Your data remains private and local to your device.
