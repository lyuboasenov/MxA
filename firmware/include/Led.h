#pragma once

#include <Arduino.h>
#include "Config.h"
#include "debug_print.h"

#ifdef LED_DEBUG_ENABLED
#define LED_DEBUG(x, y,...)                DEBUG_OUTPUT(x, "LED", y, ##__VA_ARGS__) //!< DEBUG
#else
#define LED_DEBUG(x, y,...)                                              //!< DEBUG null
#endif

class Led {
   public:
   Led() { }
   void begin();
   void blink_red();
   void turn_off_red();
   void turn_on_red();
   void blink_green();
   void turn_on_green();
   void turn_off_green();

   private:
   TaskHandle_t task_handle = NULL;
};