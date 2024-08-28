﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MxA.Helpers.Permissions;
using Plugin.Permissions.Abstractions;

namespace MxA.Droid {
   /*
    * MxA.Android/Properties/AndroidManifest.xml should contain the following permissions, which get deleted by Visual Studio !!!
    * This file should be edited manually and not using the project properties wizard.
    
  	<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
	<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />
	<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

    */
   public class BLEPermission : Xamarin.Essentials.Permissions.BasePlatformPermission, IBLEPermission {
      public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)> {
           (Android.Manifest.Permission.BluetoothAdmin, true),
           (Android.Manifest.Permission.Bluetooth, true),
           ("android.permission.BLUETOOTH_SCAN", true),
           ("android.permission.BLUETOOTH_ADVERTISE", true),
           ("android.permission.BLUETOOTH_CONNECT", true)
       }.ToArray();
   }
}