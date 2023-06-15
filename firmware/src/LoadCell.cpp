#include "LoadCell.h"

LoadCell::LoadCell() {}

void LoadCell::begin() {

   _prefs.begin("HX711");

   _scale_factor = _prefs.getFloat("scale_factor", _default_scale_factor);

   // configure load cell XTAL PWM
   ledcSetup(LOAD_CELL_XTAL_CHANNEL, LOAD_CELL_XTAL_FREQ, 2);
   ledcAttachPin(LOAD_CELL_XTAL_PIN, LOAD_CELL_XTAL_CHANNEL);
   ledcWrite(LOAD_CELL_XTAL_CHANNEL, 1);

   pinMode(LOAD_CELL_XTAL_ENABLE_PIN, OUTPUT);
   digitalWrite(LOAD_CELL_XTAL_ENABLE_PIN, HIGH);

   _hx_711.begin(LOAD_CELL_DOUT_PIN, LOAD_CELL_SCK_PIN);

   _hx_711.set_scale(_scale_factor);  // this value is obtained by calibrating the scale with known weights; see the README for details
   _hx_711.tare();                   // reset the scale to 0
}

float LoadCell::get_units(byte times) {
   return _hx_711.get_units(times);
}

void LoadCell::set_units(float value) {
   if (value == 0) {
      _hx_711.tare();
   } else {
      float scale = _hx_711.get_value() / value;
      if (scale != 0) {
         _prefs.putFloat("scale_factor", scale);
         _scale_factor = scale;
         _hx_711.set_scale(_scale_factor);
      }

   }
}