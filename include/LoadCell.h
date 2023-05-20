#pragma once

#include "HX711.h"

#ifndef LOAD_CELL_DOUT_PIN
#define LOAD_CELL_DOUT_PIN 16
#endif

#ifndef LOAD_CELL_SCK_PIN
#define LOAD_CELL_SCK_PIN 4
#endif

#ifndef LOAD_CELL_XTAL_PIN
#define LOAD_CELL_XTAL_PIN 17
#endif

#ifndef LOAD_CELL_XTAL_FREQ
#define LOAD_CELL_XTAL_FREQ 20000000
#endif

#ifndef LOAD_CELL_XTAL_CHANNEL
#define LOAD_CELL_XTAL_CHANNEL 0
#endif

#ifndef LOAD_CELL_XTAL_ENABLE_PIN
#define LOAD_CELL_XTAL_ENABLE_PIN 5
#endif

class LoadCell {
   public:
   LoadCell();
   void begin();

   float get_units(byte times = 1);

   private:
   const float _scale_factor = 44300.f;

   HX711 _hx_711;

};