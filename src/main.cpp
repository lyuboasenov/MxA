#include <Arduino.h>
#include "HX711.h"

#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>

const float scale_factor = 44300.f;

// HX711 circuit wiring
const int LOAD_CELL_DOUT_PIN = 16;
const int LOAD_CELL_SCK_PIN = 4;

// setting PWM properties
const int freq = 20000000;
const int ledChannel = 0;
const int resolution = 2;
const int ledPin = 17; // 17 corresponds to GPIO17

HX711 scale;

#define SERVICE_UUID        "6bfb42bd-7543-4856-a764-bed8a125adf2"
#define CHARACTERISTIC_UUID "da7c062b-90ff-41c4-a009-9bfefb0928ee" // 18fe53d5-7040-4096-95c1-be3c16501dc2

bool deviceConnected = false;

//Setup callbacks onConnect and onDisconnect
class MyServerCallbacks: public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    deviceConnected = true;
  };
  void onDisconnect(BLEServer* pServer) {
    deviceConnected = false;
  }
};

// 0x181D - Weight Scale
// 0x1826 - Fitness Machine
BLECharacteristic weightCharacteristics(CHARACTERISTIC_UUID, BLECharacteristic::PROPERTY_NOTIFY);
BLEDescriptor weightDescriptor(BLEUUID((uint16_t)0x181D));

void setup() {
   Serial.begin(115200);

   // configure LED PWM functionalitites
   ledcSetup(ledChannel, freq, resolution);
   ledcAttachPin(ledPin, ledChannel);
   ledcWrite(ledChannel, 1);

   pinMode(5, OUTPUT);
   digitalWrite(5, HIGH);

   scale.begin(LOAD_CELL_DOUT_PIN, LOAD_CELL_SCK_PIN);

   Serial.println("Before setting up the scale:");
   Serial.print("read: \t\t");
   Serial.println(scale.read());			// print a raw reading from the ADC

   Serial.print("read average: \t\t");
   Serial.println(scale.read_average(20));  	// print the average of 20 readings from the ADC

   Serial.print("get value: \t\t");
   Serial.println(scale.get_value(5));		// print the average of 5 readings from the ADC minus the tare weight (not set yet)

   Serial.print("get units: \t\t");
   Serial.println(scale.get_units(5), 1);	// print the average of 5 readings from the ADC minus tare weight (not set) divided
                     // by the SCALE parameter (not set yet)

   scale.set_scale(scale_factor);                      // this value is obtained by calibrating the scale with known weights; see the README for details
   scale.tare();				        // reset the scale to 0

   Serial.println("After setting up the scale:");

   Serial.print("read: \t\t");
   Serial.println(scale.read());                 // print a raw reading from the ADC

   Serial.print("read average: \t\t");
   Serial.println(scale.read_average(20));       // print the average of 20 readings from the ADC

   Serial.print("get value: \t\t");
   Serial.println(scale.get_value(5));		// print the average of 5 readings from the ADC minus the tare weight, set with tare()

   Serial.print("get units: \t\t");
   Serial.println(scale.get_units(5), 1);        // print the average of 5 readings from the ADC minus tare weight, divided
                     // by the SCALE parameter set with set_scale

   BLEDevice::init("Portable scale");
   BLEServer *pServer = BLEDevice::createServer();
   pServer->setCallbacks(new MyServerCallbacks());

   BLEService *pService = pServer->createService(SERVICE_UUID);
   pService->addCharacteristic(&weightCharacteristics);
   weightDescriptor.setValue("Weight in kg");
   weightCharacteristics.addDescriptor(&weightDescriptor);

   pService->start();
   // BLEAdvertising *pAdvertising = pServer->getAdvertising();  // this still is working for backward compatibility
   BLEAdvertising *pAdvertising = BLEDevice::getAdvertising();
   pAdvertising->addServiceUUID(SERVICE_UUID);
   pAdvertising->setScanResponse(true);
   pAdvertising->setMinPreferred(0x06);  // functions that help with iPhone connections issue
   pAdvertising->setMinPreferred(0x12);
   BLEDevice::startAdvertising();

   Serial.println("Characteristic defined! Now you can read it in your phone!");
}

long value;
float units;
void loop() {
   Serial.print(millis());
   Serial.print(" ");
   units = scale.get_units(5);

   Serial.println(units, 2);
   if (deviceConnected) {

      weightCharacteristics.setValue(units);
      weightCharacteristics.notify();
   } else {

   }
   // if (scale.is_ready()) {
   //    value = scale.read();
   //    Serial.println((float)value / 24, 2);
   // }
}
