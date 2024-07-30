#pragma once

#include <Arduino.h>

#ifdef ESP32
#include <mutex>
#endif

#ifndef VERBOSITY
#define VERBOSITY 0
#endif

typedef enum {
   error = 1,
   info,
   warning,
   debug,
   trace
} verbosity_t;

#define MY_SERIAL_OUTPUT_SIZE 120u

#ifdef DEBUG_OUTPUT_ENABLED
#define DEBUG_OUTPUT(lvl, pref, fmt, ...)                       debug_print_ln(lvl, pref, fmt, ##__VA_ARGS__) //!< DEBUG
#define DEBUG_OUTPUT_F(lvl, pref, fmt, ...)                     debug_print(lvl, pref, fmt, ##__VA_ARGS__)    //!< DEBUG
#else
#define DEBUG_OUTPUT(lvl, pref, fmt, ...)                                                                     //!< DEBUG null
#define DEBUG_OUTPUT_F(lvl, pref, fmt, ...)                                                                   //!< DEBUG null
#endif

#ifdef DUMP_BUFFER_ENABLED
#define DUMP_BUFFER(lvl, pref, buffer, size)                 dump_buffer(lvl, pref, buffer, size)             //!< DEBUG
#else
#define DUMP_BUFFER(lvl, pref, buffer, size)                                                                  //!< DEBUG null
#endif

#ifdef ESP32
static std::mutex _mutex;
#endif

void debug_print(verbosity_t level, const char *pref, const char *fmt, ...);
void debug_print_ln(verbosity_t level, const char *pref, const char *fmt, ...);
void dump_buffer(verbosity_t level, const char *pref, const uint8_t *buffer, size_t size);
