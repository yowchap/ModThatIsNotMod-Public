# ModThatIsNotMod
This is a BONEWORKS mod for loading in custom items, making life easier for other mod creators, and a whole lot more.

Feel free to make a PR for new features and bug fixes, just make sure the code is commented/easily understandable, and doesn't noticeably affect performance.

## Features
- Load custom items into the game
- A custom menu that other mods can use
- Eject mags with the press of a button
- Easily get references to the player, hands, and controllers
- Get the currently held objects
- Events for when guns fire, objects are grabbed, the player dies, etc
- Replace dummy shaders with the real ingame versions
- Spawn items from pools
- Check if mods are up to date
- Send notifications to the player
- Access methods with Nullable<T> arguments that are normally broken
- Much more

### Custom Items
Place `.melon` files into `UserData/CustomItems` and they will be loaded automatically. If you don't have those folders, either manually create them or run the game once with this mod installed.

### BoneMenu
This is a custom menu that can be used by other mods. It has some basic default functionality for giving yourself ammo and spawning random custom items.
You can open and close BoneMenu either through the radial menu or a hand gesture. Using the radial menu is the default option for new installs, but I personally find the gesture nicer to use once you're used to it.

<br>

## User Preferences
You will have to run the game once with the mod installed before the preferences will show up.

Preferences are stored in `UserData/ModThatIsNotMod.cfg`.

#### Mag eject prefs
- MagEjectButtonEnabled: Whether or not the menu button should be used to eject mags from guns.
- PressesToEjectMag: How many times you have to press the menu button to eject a mag, if that feature is enabled.
- AutoEjectEmptyMags: Should mags be automatically ejected once they're empty.
- OverrideMagEjectSettings: Custom guns can force disable the mag eject button or force enable/disable auto ejecting. This will ignore that and always use your preferences.

#### BoneMenu prefs
- BoneMenuGestureEnabled: Enable to open BoneMenu by pointing your both palms up and slightly forward, and close it by pointing whichever hand it's attached to, by default your left, down and slightly forwards.
- BoneMenuRadialButtonEnabled: Lets you open BoneMenu from the radial menu.
- MenuOnRightHand: Enable this if you want the menu to follow your right hand instead of your left.
- MenuOffsetX, MenuOffsetY, and MenuOffsetZ: Allows you to change the position of the menu relative to your hand.

#### Notification prefs
- NotificationsOnRightHand: Enable this if you want notifications to follow your right hand instead of your left.
- NotificationOffsetX, NotificationOffsetY, and NotificationOffsetZ: Changes the position of the notifications relative to your hand.

#### Misc prefs
- AutomaticSpawnGuns: Will make the utility gun fully automatic.
- UtilGunInRadialMenu: Always have an option to spawn the utility gun from the radial menu, even in levels that usually don't allow it.
- InfiniteAmmoGuns: You can put custom gun names in this list to add a variant of them with infinite ammo to the spawn menu.

#### Meme prefs
- TabloidMode: Adds fun stuff to the screen outside of VR.
- HappyFunTime: Spawns fake ads or pictures of dogs occasionally.
- HappyFunTimeDelay: The min/max time between spawning a new ad or dog pic.

#### Debug prefs
- LoggingMode: Controls how much info from this mod is displayed in the console. The four options are "MINIMAL", "NORMAL", "VERBOSE", and "DEBUG".
- AllowEmbeddedCode: Some custom items have a dll embedded in them for extra functionality and this will let that code be run.
- ExportEmbeddedAssemblies: This will save any embedded dlls into `UserData/ExportedAssemblies`
- SilenceUnhollowerWarnings: Hide warnings about unsupported return types and parameters from the assembly unhollower
- ReloadItemsOnLevelChange: Reload any modified .melon files when you load into a new level, useful for quickly testing changes to your items without having to close the game.

<br>

## Credits

Chromium - Helped me create Boneworks Modding Toolkit and Custom Items Framework, which this mod is based off of

L4rs - Added a color element to BoneMenu and made a way for a menu to be attached to an item

Trev - Modified the CustomMonobehaviourHandler to support custom json serializers
  
WNP78 - Added `BoxedNullable` and `NullableMethodExtensions`

And thanks to anyone who tested, told me about bugs, or suggested new features for the mod!

<br>

## Changelogs

#### v0.2.7:
- Added a notification system
- Moved preferences from `UserData/MelonPreferences.cfg` to `UserData/ModThatIsNotMod.cfg` (your existing preferences will be moved automatically)
- Lots of code cleanup

#### v0.2.6:
- Updated to use MelonLoader v0.5.2
- Logs are now under "[MTINM]" and "[MTINM_DEBUG]" instead of "[ModThatIsNotMod]" and "[ModThatIsNotMod] [DEBUG]"
- Some other code cleanup that shouldn't affect anything
- Now open source

#### v0.2.5:
- Added an override to `CustomMonoBehaviourHandler.AddMonoBehavioursFromJson()` to allow mods to use custom JsonSerializerSettings (Thanks to @trev#1477)

#### v0.2.4:
- Added `CustomItems.LoadItemsFromBundle()` for other mods to load custom items at runtime
- Hopefully fixed some occasional errors from CycleFireModeGrips when items are loading
- Made more functions in the MenuManager class public
- Added `RegisterAndReturnAllInAssembly()`, which is the same as `RegisterAllInAssembly()` but will return an array of all the registered types
- Added some more specific errors when melon files fail to load to help creators
- Added a few debug messages for when things go wrong with custom monobehaviours being loaded from json
- Fixed file paths being relative to the exe for compatibility with the mod manager when it releases

#### v0.2.3:
- Updated version checking to use the Thunderstore API

#### v0.2.2:
- Improved EasyMenu backwards compatibility so that more mods, including Utilities, work with BoneMenu
- Changed the default method for opening BoneMenu to the radial menu instead of the gesture
- Temporarily disabled version checking until I switch it to use the Thunderstore API
- Made `CustomMonoBehaviourHandler.AddMonoBehavioursFromJson()` public so other mods can access it

#### v0.2.1:
- Added modprefs for opening bonemenu with the gesture and opening it through the radial menu (default is gesture so if you want to change it, run the game once and the modprefs will show up)
- Added a modpref for always having the utility gun in the radial menu
- Fixed ejected cases always showing the bullet
- Fixed an issue where some duplicate items (usually map portals) would show up in the spawn menu
- Fixed items not showing up if you haven't unlocked anything in their category (haven't actually tested this but it should work)
- Added a way for items to have menus attached to them (@L4rs#6912 did this)
- Added a color element for bonemenu (also @L4rs#6912)
- Made some custom monobehaviours public
- Other bug fixes

#### v0.2.0:
- Added Bonemenu
- Fixed an issue that affects both vanilla and custom guns where the slide release sound won't play if you pull the slide right after shooting
- Added individual shell loading for guns like shotguns or revolvers
- The PumpSlideGrip used on shotguns will lock if a shell is chambered
- Added a way to change how much ammo a gun uses when you fire it
- Added a way for guns to require charging before firing
- Fixed the shotgun custom monobehaviour so it can be used on new shotguns instead of MultipleFirePoints
- Added a way for custom guns to disable the slide locking back so you always have to pull it
- Changed the LoggingMode pref to use names instead of numbers, so now the options are "MINIMAL", "NORMAL", "VERBOSE", and "DEBUG"
- StabSlash, AmbientSFX, and AudioSource components on custom items now use the correct audio mixer
- Added an event for when a SpawnableObject is created and one for when something is added to the spawn menu
- Fixed an issue with loading a few old items
- Added a ModPref class for an easier way to create and get/set values in MelonPreferences
- Added backwards compatibility for mag eject settings on custom items and added a custom monobehaviour for the new version of it
- Fixed an issue with BMT backwards compatibility
- Made sure this mod will always load first
- Fixed a rare issue where guns would eject shells with the wrong size
- Replaced Min/MaxHappyFunTimeWait prefs with one pref called HappyFunTimeDelay
- Added a chance for a dog picture to spawn when HappyFunTime is enabled
- Yeeted the disclaimer into the void \o/
- Other small changes that nobody will care about

#### v0.1.2:
- Added a modpref to reload items whenever you load into a new scene (should be much faster than with the original custom items mod)
- Added backwards compatibility for guns that change the ejected bullet scale
- Fixed issue where some custom items couldn't be despawned
- Fixed an occasional error when loading .melon files
- A few small misc. changes

#### v0.1.1:
- Hopefully fixed an issue with mag eject input not being detected for some oculus users

#### v0.1.0:
- Initial beta release
