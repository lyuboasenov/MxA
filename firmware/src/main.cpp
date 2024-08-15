#include <Arduino.h>

#include "Ble.h"
#include "LoadCell.h"
#include "BatteryLevel.h"
#include "Led.h"
#include "debug_print.h"

Ble _ble;
LoadCell _load_cell;
BatteryLevel _battery;
Led _led;

void setup() {
   #ifdef DEBUG_OUTPUT_ENABLED
   Serial.begin(115200);
   #endif

   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  _________________________________");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", " ))       MxA v.%d.%d.%d.%d (( ", SOFTWARE_VERSION_MAJOR, SOFTWARE_VERSION_MINOR, SOFTWARE_VERSION_PATCH, SOFTWARE_VERSION_BUILD);
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "((                                  ))");
   DEBUG_OUTPUT(verbosity_t::info, "MAIN", "  ----------------------------------  ");

   _load_cell.begin();
   _ble.begin();
   _battery.begin();
   _led.begin();

   _led.blink_red();
}

uint32_t last_battery_checked_at = 0;
float units;
void loop() {

   _ble.check_to_reconnect();

   if (_ble.had_received_weight()) {
      float received_weight = _ble.read_received_weight();
      _load_cell.set_units(received_weight);
   }

   units = _load_cell.get_units(5);

   if (_ble.connected()) {
      _ble.weight_notify(units);

      if (millis() - last_battery_checked_at > BATTERY_LEVEL_CHECK_INTERVAL_MILLIS) {
         _ble.battery_notify(_battery.get_level());
         last_battery_checked_at = millis();
      }

      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "%d %d", millis(), units);

      _led.turn_off_red();
      _led.blink_green();
   } else {
      DEBUG_OUTPUT(verbosity_t::debug, "MAIN", "Waiting for client");
      _led.blink_red();
      _led.turn_off_green();
   }
}
