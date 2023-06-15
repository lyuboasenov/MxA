#include "debug_print.h"

const char * get_level(verbosity_t lvl) {
   switch (lvl) {
      case verbosity_t::error:
         return "ERROR";
      case verbosity_t::info:
         return "INFO ";
      case verbosity_t::debug:
         return "DEBUG";
      case verbosity_t::trace:
         return "TRACE";
      case verbosity_t::warning:
         return "WARN ";
      default:
         return "     ";
   }
}

void debug_print(verbosity_t level, const char *pref, const char *fmt, ...) {
#if !defined(DISABLED_SERIAL) && VERBOSITY > 0
   if (level <= VERBOSITY) {
      #ifdef ESP32
      std::lock_guard<std::mutex> lck(_mutex);
      #endif

      char fmtBuffer[MY_SERIAL_OUTPUT_SIZE];

      va_list args;
      va_start(args, fmt);
      vsnprintf_P(fmtBuffer, sizeof(fmtBuffer), fmt, args);
      va_end(args);
      Serial.printf("%s:", pref);
      Serial.print(fmtBuffer);
      Serial.flush();
   }
#else
   (void)fmt;
#endif
}

void debug_print_ln(verbosity_t level, const char *pref, const char *fmt, ...) {
#if !defined(DISABLED_SERIAL) && VERBOSITY > 0
   if (level <= VERBOSITY) {
      #ifdef ESP32
      std::lock_guard<std::mutex> lck(_mutex);
      #endif

      char fmtBuffer[MY_SERIAL_OUTPUT_SIZE];

      // prepend timestamp
      Serial.printf("[% 10u] [%s] %s:", millis(), get_level(level), pref);
      va_list args;
      va_start(args, fmt);
      vsnprintf_P(fmtBuffer, sizeof(fmtBuffer), fmt, args);
      va_end(args);
      Serial.print(fmtBuffer);
      Serial.println();
      Serial.flush();
   }
#else
   (void)fmt;
#endif
}

void dump_buffer(verbosity_t level, const char *pref, const uint8_t *buffer, size_t size) {
#if !defined(DISABLED_SERIAL) && VERBOSITY > 0
   if (level <= VERBOSITY) {
      #ifdef ESP32
      std::lock_guard<std::mutex> lck(_mutex);
      #endif

      Serial.printf("[% 10u] [%s] %s %s", millis(), get_level(level), pref, " BUFFER:[");

      for(size_t i = 0; i < size; i++) {
         Serial.printf("0x%02x, ", buffer[i]);
      }
      Serial.println("]");
   }
#else
   (void)pref;
#endif
}