#include "Ble.h"

Ble::Ble() :
   _device_name_descriptor(BLEUUID((uint16_t)0x2901)),
   _device_name_characteristics(
      BLEUUID((uint16_t) DEVICE_NAME_CHARACTERISTIC_UUID),
      BLECharacteristic::PROPERTY_READ),
   _weight_descriptor(BLEUUID((uint16_t)0x2901)),
   _weight_characteristics(
      BLEUUID((uint16_t) WEIGHT_CHARACTERISTIC_UUID),
      BLECharacteristic::PROPERTY_NOTIFY |
      BLECharacteristic::PROPERTY_WRITE |
      BLECharacteristic::PROPERTY_READ),
   _battery_level_descriptor(BLEUUID((uint16_t)0x2901)),
   _battery_level_characteristics(
      BLEUUID((uint16_t) BATTERY_CHARACTERISTIC_UUID),
      BLECharacteristic::PROPERTY_NOTIFY |
      BLECharacteristic::PROPERTY_READ)
    {}

void Ble::begin() {

   BLEDevice::init(DEVICE_NAME);
   _server = BLEDevice::createServer();
   _server->setCallbacks(new MyServerCallbacks(this));

   BLEService *weightService = _server->createService(BLEUUID((uint16_t) WEIGHT_SERVICE_UUID));
   weightService->addCharacteristic(&_weight_characteristics);
   _weight_descriptor.setValue("Weight in kilograms measured by the device");
   _weight_characteristics.addDescriptor(&_weight_descriptor);

   BLEService *batteryService = _server->createService(BLEUUID((uint16_t) BATTERY_LEVEL_SERVICE_UUID));
   batteryService->addCharacteristic(&_battery_level_characteristics);
   _battery_level_descriptor.setValue("Weight in kilograms measured by the device");
   _battery_level_characteristics.addDescriptor(&_weight_descriptor);

   weightService->addCharacteristic(&_device_name_characteristics);
   batteryService->addCharacteristic(&_device_name_characteristics);
   _device_name_descriptor.setValue(DEVICE_NAME);
   _device_name_characteristics.addDescriptor(&_device_name_descriptor);

   weightService->start();
   batteryService->start();

   BLEAdvertising *weightAdvertising = BLEDevice::getAdvertising();
   weightAdvertising->addServiceUUID(BLEUUID((uint16_t) WEIGHT_SERVICE_UUID));
   weightAdvertising->setScanResponse(true);
   weightAdvertising->setMinPreferred(0x06);  // functions that help with iPhone connections issue
   weightAdvertising->setMinPreferred(0x12);

   BLEAdvertising *weightAdvertising = BLEDevice::getAdvertising();
   weightAdvertising->addServiceUUID(BLEUUID((uint16_t) BATTERY_LEVEL_SERVICE_UUID));
   weightAdvertising->setScanResponse(true);
   weightAdvertising->setMinPreferred(0x06);  // functions that help with iPhone connections issue
   weightAdvertising->setMinPreferred(0x12);

   BLEDevice::startAdvertising();
}

void Ble::check_to_reconnect() {
   // disconnected so advertise
   if (!_device_connected && _old_device_connected) {
      delay(500);                   // give the bluetooth stack the chance to get things ready
      _server->startAdvertising();  // restart advertising
      BLE_DEBUG(verbosity_t::info, "DISCONNECT");
      _old_device_connected = _device_connected;
   }
   // connected so reset boolean control
   if (_device_connected && !_old_device_connected) {
      // do stuff here on connecting
      BLE_DEBUG(verbosity_t::info, "RE-CONNECT");
      _old_device_connected = _device_connected;
   }
}

bool Ble::connected() {
   return _device_connected;
}

void Ble::weight_notify(float value) {
   double double_value = (double) value;
   _weight_characteristics.setValue(double_value);
   _weight_characteristics.notify();
}

void Ble::battery_notify(uint16_t value) {
   _battery_level_characteristics.setValue(value);
   _battery_level_characteristics.notify();
}

void Ble::on_connect(BLEServer * server) {
   if (server == _server) {
      _device_connected = true;
   }
}

void Ble::on_disconnect(BLEServer * server) {
   if (server == _server) {
      _device_connected = false;
   }
}