# MagicDeckStats

An application for analyzing _Magic: The Gathering_ deck statistics and gameplay data, importing data from [BGStats](https://www.bgstatsapp.com/). This is a Blazor WebAssembly (WASM) application built with .NET 9.

I mainly use it to track performance of Battle Decks and see how balanced they are against each other.

**🌐 [Demo](https://rickardni.github.io/MagicDeckStats/)**

## FAQ

### **How do I import data to this app?**
You need to export data first from BGStats:

1. Open BGStats on your device
2. Go to **"Settings"**
3. Select **"Export, import and backup"**
4. Select **"Export backup file..."**
5. Choose a location to save the file

This will give you a file called `BGStatsExport.json` that you can upload to the site on the **"Import / Export"** page.

### **What are Battle Decks?**

Battle Decks are, in my opinion, the by far best way to play casual constructed Magic: The Gathering.

I plan to write a long guide on how to get started in this wonderful "format", but briefly explained, they are very affordable pre-constructed decks that Card Kingdom provides (they build and sell them), mostly made up of cheap and otherwise unwanted cards. They are balanced to have approximately the same power level, so they can be mixed and matched against each other without many problems. In fact, this app was created just to demonstrate how well-balanced they are. Any meta in Magic: The Gathering will have decks with higher or lower win rates - that's normal and expected, so with that in mind, it's amazing how well they have balanced these decks.

You can also find more information about actual decks here:  
https://sites.google.com/view/profslittlesite/home

### **Why is the code shit?**
I have "vibe coded" this whole app because I'm disgusting.

On a more serious note, this was a test to see how fast I could get an app up-and-running with the help of Cursor. The code might not always be the best, and some tweaking was necessary, but overall I'm happy with the result, considering the time spent.

### **Is my data stored on a server?**
No, this is a client-side application. All data processing happens in your browser, and no data is sent to external servers. Your data remains private and local to your device.

### **What do the labels 'Tour Winner' and 'Finalist' mean?**
Tour Winner means the deck won a Swiss 3 round tournament with 3 - 0 (3 wins and 0 losses).

Finalist means the deck has otherwise reached a true final in a Swiss 3 round tournament where the finalists were both 2 - 0 at that point.
(A tournament with 6 players, for instance, can have a "final" that is not necessarily with two undefeated players.)

