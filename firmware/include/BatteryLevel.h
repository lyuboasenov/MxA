#pragma once

#include "Arduino.h"
#include "Config.h"

class BatteryLevel {
   public:
   BatteryLevel();
   void begin();

   uint16_t get_level();
};