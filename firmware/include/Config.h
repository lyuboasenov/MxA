/**
 * @def LOAD_CELL_DOUT_PIN
 * @brief The ESP32 pin connected to HX711 DOUT pin.
 */
#ifndef LOAD_CELL_DOUT_PIN
#define LOAD_CELL_DOUT_PIN 18
#endif

/**
 * @def LOAD_CELL_SCK_PIN
 * @brief The ESP32 pin connected to HX711 PD_SCK pin.
 */
#ifndef LOAD_CELL_SCK_PIN
#define LOAD_CELL_SCK_PIN 19
#endif

/**
 * @def LOAD_CELL_XTAL_PIN
 * @brief The ESP32 pin connected to HX711 XI pin.
 */
#ifndef LOAD_CELL_XTAL_PIN
#define LOAD_CELL_XTAL_PIN 5
#endif

/**
 * @def LOAD_CELL_XTAL_FREQ
 * @brief The frequency fed to HX711 in Hz. Values 1 - 20MHz.
 */
#ifndef LOAD_CELL_XTAL_FREQ
#define LOAD_CELL_XTAL_FREQ 20000000
#endif

/**
 * @def LOAD_CELL_XTAL_CHANNEL
 * @brief The [ESP timer channel](https://docs.espressif.com/projects/esp-idf/en/latest/esp32/api-reference/peripherals/ledc.html) used to generate the clock signal for HX711. Valid values 0-7.
 */
#ifndef LOAD_CELL_XTAL_CHANNEL
#define LOAD_CELL_XTAL_CHANNEL 0
#endif

/**
 * @def LOAD_CELL_XTAL_ENABLE_PIN
 * @brief The ESP32 pin connected to HX711 RATE pin.
 */
#ifndef LOAD_CELL_XTAL_ENABLE_PIN
#define LOAD_CELL_XTAL_ENABLE_PIN 17
#endif

/**
 * @def BATTERY_LEVEL_PIN
 * @brief The ESP32 pin used to measure the battery level.
 */
#ifndef BATTERY_LEVEL_PIN
#define BATTERY_LEVEL_PIN 4
#endif

/**
 * @def BATTERY_LEVEL_MAX_VALUE_ADC
 * @brief The ADC value ESP32 will measure when the battery is full.
 */
#ifndef BATTERY_LEVEL_MAX_VALUE_ADC
#define BATTERY_LEVEL_MAX_VALUE_ADC 2786
#endif

/**
 * @def BATTERY_LEVEL_MIN_VALUE_ADC
 * @brief The ADC value ESP32 will measure when the battery is empty.
 */
#ifndef BATTERY_LEVEL_MIN_VALUE_ADC
#define BATTERY_LEVEL_MIN_VALUE_ADC 1725
#endif

/**
 * @def BATTERY_LEVEL_CHECK_INTERVAL_MILLIS
 * @brief The interval for measuring battery level in milliseconds.
 */
#ifndef BATTERY_LEVEL_CHECK_INTERVAL_MILLIS
#define BATTERY_LEVEL_CHECK_INTERVAL_MILLIS 2000
#endif

/**
 * @def LED_RED_PIN
 * @brief The ESP32 pin connected to the red led.
 */
#ifndef LED_RED_PIN
#define LED_RED_PIN 23
#endif

/**
 * @def LED_GREEN_PIN
 * @brief
 */
#ifndef LED_GREEN_PIN
#define LED_GREEN_PIN 22
#endif

/**
 * @def LED_FADE_AMOUNT
 * @brief The value by which led intensity is altered when fading.
 */
#ifndef LED_FADE_AMOUNT
#define LED_FADE_AMOUNT 5
#endif