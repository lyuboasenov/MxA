#include <Arduino.h>

#include "Ble.h"
#include "LoadCell.h"
#include "BatteryLevel.h"
#include "debug_print.h"

Ble _ble;
LoadCell _load_cell;
BatteryLevel _battery;

void setup() {
   #ifdef DEBUG_OUTPUT_ENABLED
   Serial.begin(115200);
   #endif

   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  _________________________________");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", " ))       MxA v. %s (( ", VERSION);
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  ----------------------------------  ");

   _load_cell.begin();
   _ble.begin();
   _battery.begin();
}

uint32_t last_battery_checked_at = 0;
float units;
void loop() {

   _ble.check_to_reconnect();

   units = _load_cell.get_units(5);

   if (_ble.connected()) {
      _ble.weight_notify(units);

      if (millis() - last_battery_checked_at > BATTERY_LEVEL_CHECK_INTERVAL_MILLIS) {
         _ble.battery_notify(_battery.get_level());
      }

      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "%d %d", millis(), units);
   } else {
      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "Waiting for client");
   }
}
