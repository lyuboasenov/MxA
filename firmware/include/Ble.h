#pragma once

#include <Arduino.h>
#include <BLEDevice.h>
#include <BLEServer.h>
#include <BLEUtils.h>
#include "debug_print.h"

#ifdef BLE_DEBUG_ENABLED
#define BLE_DEBUG(x, y,...)                DEBUG_OUTPUT(x, "BLE", y, ##__VA_ARGS__) //!< DEBUG
#else
#define BLE_DEBUG(x, y,...)                                              //!< DEBUG null
#endif

#define DEVICE_NAME "[MxA] Portable"

#define WEIGHT_SERVICE_UUID 0x181D                           // Weight Scale
#define WEIGHT_CHARACTERISTIC_UUID 0x2A98
#define DEVICE_NAME_CHARACTERISTIC_UUID 0x2A00

#define BATTERY_LEVEL_SERVICE_UUID 0x180F
#define BATTERY_CHARACTERISTIC_UUID 0x2A19

class Ble {
   public:
   Ble();
   void begin();

   void check_to_reconnect();

   void on_connect(BLEServer * server);
   void on_disconnect(BLEServer * server);

   bool connected();

   void weight_notify(float value);
   void battery_notify(uint16_t value);

   private:
   BLEServer * _server = NULL;
   bool _device_connected = false;
   bool _old_device_connected = false;

   BLECharacteristic _weight_characteristics;
   BLEDescriptor _weight_descriptor;

   BLECharacteristic _device_name_characteristics;
   BLEDescriptor _device_name_descriptor;

   BLECharacteristic _battery_level_characteristics;
   BLEDescriptor _battery_level_descriptor;
};

// Setup callbacks onConnect and onDisconnect
class MyServerCallbacks : public BLEServerCallbacks {
   void onConnect(BLEServer *pServer) {
      _ble->on_connect(pServer);
   };

   void onDisconnect(BLEServer *pServer) {
      _ble->on_disconnect(pServer);
   }

   public:
   MyServerCallbacks(Ble* ble) : _ble(ble) { }

   private:
   Ble * _ble;
};