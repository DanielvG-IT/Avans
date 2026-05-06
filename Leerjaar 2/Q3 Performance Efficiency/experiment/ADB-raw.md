reset the phone
no google account
no location sharing
no password
no pattern lock
no fingerprint lock
no face recognition lock
no sim card
no samsung account
no samsung services
no recommended apps installed

developer options enabled
usb debugging enabled

danielvginneken@MacBookPro ~ % adb devices  
List of devices attached
RZCT401NHBN device

danielvginneken@MacBookPro ~ % adb shell getprop ro.product.model
SM-A536B

danielvginneken@MacBookPro ~ % adb shell getprop ro.build.fingerprint
samsung/a53xnaeea/a53x:16/BP2A.250605.031.A3/A536BXXSKGZC1:user/release-keys

danielvginneken@MacBookPro ~ % adb shell getprop ro.build.version.incremental
A536BXXSKGZC1

danielvginneken@MacBookPro ~ % adb shell getprop ro.build.version.release
adb shell getprop ro.build.version.sdk
adb shell getprop ro.build.version.security_patch
16
36
2026-03-05

danielvginneken@MacBookPro ~ % adb shell settings put global auto_time 0
adb shell settings put global auto_time_zone 0
adb shell settings put global development_settings_enabled 1

danielvginneken@MacBookPro ~ % adb shell svc data disable
adb shell svc bluetooth disable
adb shell svc nfc disable
BluetoothShellCommand: Exec disable
disable: Success

danielvginneken@MacBookPro ~ % adb shell settings put system screen_brightness_mode 0
adb shell settings put system screen_brightness 120

danielvginneken@MacBookPro ~ % adb shell settings put global stay_on_while_plugged_in 3

danielvginneken@MacBookPro ~ % adb shell settings put global window_animation_scale 0
adb shell settings put global transition_animation_scale 0
adb shell settings put global animator_duration_scale 0

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.samsung.android.bixby.agent
adb shell pm uninstall --user 0 com.samsung.android.bixby.service
adb shell pm uninstall --user 0 com.samsung.android.app.spage
adb shell pm uninstall --user 0 com.samsung.android.samsungpass
adb shell pm uninstall --user 0 com.samsung.android.samsungpassautofill
adb shell pm uninstall --user 0 com.samsung.android.scloud
adb shell pm uninstall --user 0 com.samsung.android.mdx
adb shell pm uninstall --user 0 com.samsung.android.game.gamehome
adb shell pm uninstall --user 0 com.samsung.android.game.gos
adb shell pm uninstall --user 0 com.samsung.android.kidsinstaller
Success
Failure [not installed for 0]
Success
Success
Success
Success
Success
Success
Success
Success

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.facebook.katana
adb shell pm uninstall --user 0 com.facebook.system
adb shell pm uninstall --user 0 com.facebook.appmanager
Success
Success
Success

google play store updates apply anonymously

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.samsung.android.app.tips
adb shell pm uninstall --user 0 com.samsung.android.smartsuggestions
adb shell pm uninstall --user 0 com.samsung.android.app.routines
adb shell pm uninstall --user 0 com.samsung.android.mdx
adb shell pm uninstall --user 0 com.samsung.android.mdx.kit
adb shell pm uninstall --user 0 com.samsung.android.mcfserver
adb shell pm uninstall --user 0 com.samsung.android.mcfds
adb shell pm uninstall --user 0 com.samsung.android.allshare.service.mediashare
adb shell pm uninstall --user 0 com.samsung.android.smartmirroring
adb shell pm uninstall --user 0 com.samsung.android.visual.cloudcore
adb shell pm uninstall --user 0 com.samsung.android.aware.service
adb shell pm uninstall --user 0 com.samsung.android.ipsgeofence
Success
Success
Success
Failure [not installed for 0]
Success
Success
Success
Success
Success
Success
Success
Success

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.sec.android.diagmonagent
adb shell pm uninstall --user 0 com.samsung.android.dqagent
adb shell pm uninstall --user 0 com.samsung.android.knox.analytics.uploader
adb shell pm uninstall --user 0 com.google.android.feedback
adb shell pm uninstall --user 0 com.google.mainline.telemetry
Success
Success
Success
Success
Success

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.google.android.apps.maps
adb shell pm uninstall --user 0 com.google.android.apps.photos
adb shell pm uninstall --user 0 com.google.android.apps.docs
adb shell pm uninstall --user 0 com.google.android.youtube
adb shell pm uninstall --user 0 com.microsoft.skydrive
adb shell pm uninstall --user 0 com.microsoft.appmanager
Success
Success
Success
Success
Success
Success

danielvginneken@MacBookPro ~ % adb shell pm disable-user --user 0 com.samsung.android.location
adb shell pm disable-user --user 0 com.android.location.fused
adb shell pm disable-user --user 0 com.google.android.gms.location.history
adb shell pm disable-user --user 0 com.google.android.gms.supervision
adb shell pm disable-user --user 0 com.google.android.gms
Package com.samsung.android.location new state: disabled-user

Exception occurred while executing 'disable-user':
java.lang.SecurityException: Cannot disable required package com.android.location.fused
at com.android.server.pm.PackageManagerService.-$$Nest$msetEnabledSettings(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:267)
at com.android.server.pm.PackageManagerService$IPackageManagerImpl.setApplicationEnabledSetting(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:92)
	at com.android.server.pm.PackageManagerShellCommand.runSetEnabledSetting(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:108)
	at com.android.server.pm.PackageManagerShellCommand.onCommand(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:1797)
	at com.android.modules.utils.BasicShellCommandHandler.exec(BasicShellCommandHandler.java:97)
	at android.os.ShellCommand.exec(ShellCommand.java:38)
	at com.android.server.pm.PackageManagerService$IPackageManagerImpl.onShellCommand(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:23)
at android.os.Binder.shellCommand(Binder.java:1187)
at android.os.Binder.onTransact(Binder.java:979)
at android.content.pm.IPackageManager$Stub.onTransact(IPackageManager.java:5301)
	at com.android.server.pm.PackageManagerService$IPackageManagerImpl.onTransact(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:1)
at android.os.Binder.execTransactInternal(Binder.java:1462)
at android.os.Binder.execTransact(Binder.java:1401)
Package com.google.android.gms.location.history new state: disabled-user

Exception occurred while executing 'disable-user':
java.lang.SecurityException: Shell cannot change component state for null to 3
at com.android.server.pm.PackageManagerService.-$$Nest$msetEnabledSettings(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:876)
at com.android.server.pm.PackageManagerService$IPackageManagerImpl.setApplicationEnabledSetting(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:92)
	at com.android.server.pm.PackageManagerShellCommand.runSetEnabledSetting(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:108)
	at com.android.server.pm.PackageManagerShellCommand.onCommand(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:1797)
	at com.android.modules.utils.BasicShellCommandHandler.exec(BasicShellCommandHandler.java:97)
	at android.os.ShellCommand.exec(ShellCommand.java:38)
	at com.android.server.pm.PackageManagerService$IPackageManagerImpl.onShellCommand(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:23)
at android.os.Binder.shellCommand(Binder.java:1187)
at android.os.Binder.onTransact(Binder.java:979)
at android.content.pm.IPackageManager$Stub.onTransact(IPackageManager.java:5301)
	at com.android.server.pm.PackageManagerService$IPackageManagerImpl.onTransact(qb/101473338 6913c9af4825d329ac2832491a383b69b47466246caa1e9612468c4954905fca:1)
at android.os.Binder.execTransactInternal(Binder.java:1462)
at android.os.Binder.execTransact(Binder.java:1401)
Package com.google.android.gms new state: disabled-user

danielvginneken@MacBookPro ~ % adb shell pm uninstall --user 0 com.google.android.apps.messaging
Success

default widgets removed from home screen

in chrome://flags/
#enable-benchmarking = "Default features states"
#enable-command-line-on-non-rooted-devices = "Enabled"

adb shell settings put system screen_off_timeout 2147483647
