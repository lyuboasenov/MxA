#pragma once

#include "HX711.h"
#include "Config.h"
#include <Preferences.h>

class LoadCell {
   public:
   LoadCell();
   void begin();

   float get_units(byte times = 1);
   void set_units(float value);

   private:
   const float _default_scale_factor = 44300.f;
   float _scale_factor = 44300.f;

   HX711 _hx_711;
   Preferences _prefs;
};