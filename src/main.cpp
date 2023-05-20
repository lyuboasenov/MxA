#include <Arduino.h>

#include "HX711.h"
#include "Ble.h"

const float scale_factor = 44300.f;

// HX711 circuit wiring
const int LOAD_CELL_DOUT_PIN = 16;
const int LOAD_CELL_SCK_PIN = 4;

// setting PWM properties
const int freq = 20000000;
const int ledChannel = 0;
const int resolution = 2;
const int ledPin = 17;  // 17 corresponds to GPIO17

HX711 scale;
Ble _ble;


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
   Serial.println(scale.read());  // print a raw reading from the ADC

   Serial.print("read average: \t\t");
   Serial.println(scale.read_average(20));  // print the average of 20 readings from the ADC

   Serial.print("get value: \t\t");
   Serial.println(scale.get_value(5));  // print the average of 5 readings from the ADC minus the tare weight (not set yet)

   Serial.print("get units: \t\t");
   Serial.println(scale.get_units(5), 1);  // print the average of 5 readings from the ADC minus tare weight (not set) divided
                                           // by the SCALE parameter (not set yet)

   scale.set_scale(scale_factor);  // this value is obtained by calibrating the scale with known weights; see the README for details
   scale.tare();                   // reset the scale to 0

   Serial.println("After setting up the scale:");

   Serial.print("read: \t\t");
   Serial.println(scale.read());  // print a raw reading from the ADC

   Serial.print("read average: \t\t");
   Serial.println(scale.read_average(20));  // print the average of 20 readings from the ADC

   Serial.print("get value: \t\t");
   Serial.println(scale.get_value(5));  // print the average of 5 readings from the ADC minus the tare weight, set with tare()

   Serial.print("get units: \t\t");
   Serial.println(scale.get_units(5), 1);  // print the average of 5 readings from the ADC minus tare weight, divided
                                           // by the SCALE parameter set with set_scale
   _ble.begin();
}

float units;
void loop() {

   _ble.check_to_reconnect();

   units = scale.get_units(5);

   if (_ble.connected()) {
      _ble.weight_notify(units);


      Serial.print(millis());
      Serial.print(" ");
      Serial.println(units, 2);
   } else {
      Serial.println("Waiting for client");
   }
}
