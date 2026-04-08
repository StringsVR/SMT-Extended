# SMT

SMT Extended is an effective and simple mod loader for the YW1 smartphone port.


<a href="https://github.com/StringsVR/SMT-Extended/releases/latest" target="_blank">
  <img alt="Download YWML" src="https://img.shields.io/github/v/release/StringsVR/SMT-Extended" width="300">
</a>

## 🔎 Obtaining .APKS/.ZIP 🔎
Yo-Kai Watch Smartphone use Google's Split Delivery system, meaning that they are packaged as .APKS and not .APK

### Lucky Patcher
This is really simple, just open lucky patcher, click export and it will save as a .APKS

## ✨ Add & Install your Mod ✨
Launch SMT, Click "Import" and select your .APKS, then after it finishes, Select the "Mods" and import your .YKM then just select install and wait for it to finish. 
After, installing your desired mods, go back to "Home" and click "Compile", the output will be a .APK that you can freely install.

## 🔨 How to create a .YKM file 🔨
A .ykm file is just a .zip, containing 2 folders and a .JSON
```
-example.ykm
    -base
    -split
    -meta.json
```

- The ``base`` folder correlates to the ``base`` folder of the apk, it stores the title of the app aswell as contains the smali code.
- The ``split`` folder correlates to the ``split_asset_time_pack_install``, this stores most of the game contents, basically the "rom".
- The ``meta.json`` holds the information for the mod, like the title, author, and version:
```json
{
    "Name": "Your mod's name",
    "Author": "Your name",
    "Version": "Your mod's version"
}
```

## 🧩 Extension System 🧩
SMT is built with a expandable and easy to implement extension system! The system allows for custom menus and access to the ``ITools``.
Simply create a .NET Maui Library and add SMT.Core to the dependencies then make a ``Entry.cs``:
```c#
    public class Entry : IPlugin
    {
        public required IMain Main { get; set; }

        public string Name => "Your Mod";
        public ContentView View {  get; set; }

        public void Initialize(IMain main)
        {

            Main = main;
            View = new smth(Main);
        }
    }
```
