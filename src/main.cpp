#include <Arduino.h>

#include "Ble.h"
#include "LoadCell.h"

Ble _ble;
LoadCell _load_cell;

void setup() {
   Serial.begin(115200);

   _load_cell.begin();
   _ble.begin();
}

float units;
void loop() {

   _ble.check_to_reconnect();

   units = _load_cell.get_units(5);

   if (_ble.connected()) {
      _ble.weight_notify(units);


      Serial.print(millis());
      Serial.print(" ");
      Serial.println(units, 2);
   } else {
      Serial.println("Waiting for client");
   }
}
