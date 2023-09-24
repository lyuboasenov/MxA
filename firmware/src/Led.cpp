#include "Led.h"
#include "freertos/FreeRTOS.h"
#include "freertos/task.h"

bool _red_led_on = false;
bool _red_led_blink = false;
bool _green_led_on = false;
bool _green_led_blink = false;
uint64_t _red_toggled = 0;
uint64_t _green_toggled = 0;

int _red_fade_amount = LED_FADE_AMOUNT;
int _green_fade_amount = LED_FADE_AMOUNT;

int _red_brightness = 0;
int _green_brightness = 0;

void led_task_handler(void *arg);

void Led::begin() {
   pinMode(LED_RED_PIN, OUTPUT);
   pinMode(LED_GREEN_PIN, OUTPUT);

   xTaskCreate(led_task_handler, "LED_Task", 4096, NULL, 10, &task_handle);
}

void Led::blink_red() {
   _red_led_on = false;
   _red_led_blink = true;
}

void Led::turn_off_red() {
   _red_led_on = false;
   _red_led_blink = false;
}

void Led::turn_on_red() {
   _red_led_on = true;
   _red_led_blink = false;
}

void Led::blink_green() {
   _green_led_blink = true;
   _green_led_on = false;
}

void Led::turn_on_green() {
   _green_led_blink = false;
   _green_led_on = true;
}

void Led::turn_off_green() {
   _green_led_blink = false;
   _green_led_on = false;
}

void led_task_handler(void *arg) {
   while(1){
      if (_red_led_on) {
         digitalWrite(LED_RED_PIN, HIGH);
      } else if (_red_led_blink) {
         analogWrite(LED_RED_PIN, _red_brightness);
         _red_brightness += _red_fade_amount;
         if (_red_brightness <= 0 || _red_brightness >= 255) {
            _red_fade_amount = -_red_fade_amount;
         }
         // if (millis() - _red_toggled > LED_RED_BLINK_PERIOD_MS) {
         //    digitalWrite(LED_RED_PIN, !digitalRead(LED_RED_PIN));
         //    _red_toggled = millis();
         // }
      } else {
         analogWrite(LED_RED_PIN, 0);
         digitalWrite(LED_RED_PIN, LOW);
      }

      if (_green_led_on) {
         digitalWrite(LED_GREEN_PIN, HIGH);
      } else if (_green_led_blink) {
         analogWrite(LED_GREEN_PIN, _green_brightness);
         _green_brightness += _green_fade_amount;
         if (_green_brightness <= 0 || _green_brightness >= 255) {
            _green_fade_amount = -_green_fade_amount;
         }
         // if (millis() - _green_toggled > LED_GREEN_BLINK_PERIOD_MS) {
         //    digitalWrite(LED_GREEN_PIN, !digitalRead(LED_GREEN_PIN));
         //    _green_toggled = millis();
         // }
      } else {
         analogWrite(LED_GREEN_PIN, 0);
         digitalWrite(LED_GREEN_PIN, LOW);
      }

      vTaskDelay(30);
   }
}