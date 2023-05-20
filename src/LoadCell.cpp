#include "LoadCell.h"

LoadCell::LoadCell() {}

void LoadCell::begin() {

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