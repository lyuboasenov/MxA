#pragma once

#include "Arduino.h"
#include "Config.h"

class BatteryLevel {
   public:
   BatteryLevel();

   /**
    * Initializes the BatteryLevel component.
    */
   void begin();

   /**
    * Gets battery level in percent.
    */
   uint16_t get_level();
};