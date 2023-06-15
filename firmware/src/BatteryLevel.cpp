#include "BatteryLevel.h"

BatteryLevel::BatteryLevel() {}

void BatteryLevel::begin() {
   pinMode(BATTERY_LEVEL_PIN, INPUT);
}

uint16_t BatteryLevel::get_level() {
   analogSetAttenuation(adc_attenuation_t::ADC_2_5db);
   uint16_t value = analogRead(BATTERY_LEVEL_PIN);

   return ((value - BATTERY_LEVEL_MIN_VALUE_ADC) * 100) / (BATTERY_LEVEL_MAX_VALUE_ADC - BATTERY_LEVEL_MIN_VALUE_ADC);
}