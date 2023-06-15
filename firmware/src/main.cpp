#include <Arduino.h>

#include "Ble.h"
#include "LoadCell.h"
#include "debug_print.h"

Ble _ble;
LoadCell _load_cell;

void setup() {
   #ifdef DEBUG_OUTPUT_ENABLED
   Serial.begin(115200);
   #endif

   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  _________________________________");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", " ))   F = MxA v. %s (( ", VERSION);
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  ----------------------------------  ");

   _load_cell.begin();
   _ble.begin();

   pinMode(16, OUTPUT);
   digitalWrite(16, HIGH);
}

float units;
void loop() {

   _ble.check_to_reconnect();

   units = _load_cell.get_units(5);

   if (_ble.connected()) {
      _ble.weight_notify(units);

      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "%d %d", millis(), units);
   } else {
      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "Waiting for client");
   }
}
