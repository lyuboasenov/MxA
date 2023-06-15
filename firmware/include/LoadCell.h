#pragma once

#include "HX711.h"
#include "Config.h"
class LoadCell {
   public:
   LoadCell();
   void begin();

   float get_units(byte times = 1);

   private:
   const float _scale_factor = 44300.f;

   HX711 _hx_711;

};