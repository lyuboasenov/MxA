# Firmware

The firmware is build using C/C++ [Arduino framework](https://www.arduino.cc/reference/en/libraries/) and [PlatformIO](https://platformio.org/).

## Firmware components

The firmware consists of these components:

### Battery level

This component is responsible for getting battery level in percent.

### BLE

This component is responsible for publishing the BLE services that broadcast the collected data and receive data for calibrating the device.

#### BLE services

The device published two BLE services.

##### Weight service - 0x181D

This service reports measured weight. Or if you write a value to this service, the calibration process is run.

##### Weight service - 0x180F

This service reports battery level.

### Load cell

This component is HAL (hardware abstraction layer) over the amplifier and bogde HX711 library.

### LED

This component is responsible for controlling the indicator LEDs.

There are two LEDs on the board. When the device is powered and is not paired the "red" LED fades in and out. When the device is paired the "red" LED stops fading and the "green" one starts fading in and out.

## Global configuration

The firmware can be customized by setting these global variables:

### LOAD_CELL_DOUT_PIN

The ESP32 pin connected to HX711 DOUT pin.

### LOAD_CELL_SCK_PIN

The ESP32 pin connected to HX711 PD_SCK pin.

### LOAD_CELL_XTAL_PIN

The ESP32 pin connected to HX711 XI pin.

### LOAD_CELL_XTAL_FREQ

The frequency fed to HX711 in Hz. Values 1 - 20MHz.

### LOAD_CELL_XTAL_CHANNEL

The [ESP timer channel](https://docs.espressif.com/projects/esp-idf/en/latest/esp32/api-reference/peripherals/ledc.html) used to generate the clock signal for HX711. Valid values 0 - 7.

### LOAD_CELL_XTAL_ENABLE_PIN

The ESP32 pin connected to HX711 RATE pin.

### BATTERY_LEVEL_PIN

The ESP32 pin used to measure the battery level.

### BATTERY_LEVEL_MAX_VALUE_ADC

The ADC value ESP32 will measure when the battery is full.

### BATTERY_LEVEL_MIN_VALUE_ADC

The ADC value ESP32 will measure when the battery is empty.

### BATTERY_LEVEL_CHECK_INTERVAL_MILLIS

The interval for measuring battery level in milliseconds.

### LED_RED_PIN

The ESP32 pin connected to the red led.

### LED_GREEN_PIN

The ESP32 pin connected to the green led.

### LED_FADE_AMOUNT

The value by which led intensity is altered when fading.

## Flashing firmware

The firmware can be flashed to the ESP32-WROOM module either through serial port (UART) or JTAG.

**Note** The first firmware flashing on a ESP32-WROOM module can only be done using the serial port [espressif docs](https://docs.espressif.com/projects/esp-idf/en/latest/esp32/api-guides/jtag-debugging/tips-and-quirks.html#jtag-and-esp32-wroom-32-at-firmware-compatibility-issue).

There is a point called:
**JTAG and ESP32-WROOM-32 AT firmware Compatibility Issue**

With this text:
**The ESP32-WROOM series of modules come pre-flashed with AT firmware. This firmware configures the pins GPIO12 to GPIO15 as SPI slave interface, which makes using JTAG impossible.**

**To make JTAG available, build new firmware that is not using pins GPIO12 to GPIO15 dedicated to JTAG communication. After that, flash the firmware onto your module.**
