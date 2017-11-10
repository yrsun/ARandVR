#include <SoftwareSerial.h>
#include <TinyGPS.h>
#include <ArduinoJson.h>

/* This sample code demonstrates the normal use of a TinyGPS object.
   It requires the use of SoftwareSerial, and assumes that you have a
   4800-baud serial GPS device hooked up on pins 4(rx) and 3(tx).
*/

TinyGPS gps;

SoftwareSerial xbee(8, 9);
SoftwareSerial ss(10, 11);

#define BUFFER_SIZE 200

unsigned long timeStamp;

void setup()
{
  Serial.begin(115200);
  while (!Serial) {
    // wait serial port initialization
  }
  ss.begin(9600);
  while (!ss) {
    // wait serial port initialization
  }
  xbee.begin(9600);
  while (!xbee) {
    // wait serial port initialization
  }
  
  Serial.print("VRAR: GPS Test using TinyGPS ver."); Serial.println(TinyGPS::library_version());
  Serial.println();
}

void loop()
{
  ss.listen();
  bool newData = false;
  unsigned long chars;
  unsigned short sentences, failed;

  // For one second we parse GPS data and report some key values
  for (unsigned long start = millis(); millis() - start < 800;)
  {
    while (ss.available())
    {
      char c = ss.read();
      
       //Serial.write("LAA"); // uncomment this line if you want to see the GPS data flowing
      if (gps.encode(c)) // Did a new valid sentence come in?
        newData = true;
    }
  }

  timeStamp = millis();

  // Allocate a temporary memory pool
  DynamicJsonBuffer jsonBuffer(BUFFER_SIZE);
  JsonObject& root = jsonBuffer.createObject();

  root["DEVICE"] = "GPS.Regular.1";
  root["TIME_STAMP"] = timeStamp;
  root["STATUS"] = "IDLE";

  root["LAT"] = 0.0;
  root["LON"] = 0.0;
  root["ALT"] = 0.0;
  root["SAT"] = 0;
  root["PREC"] = 0;
  
  if (newData)
  {
    float flat, flon;
    unsigned long age;
    gps.f_get_position(&flat, &flon, &age);

    root["STATUS"] = "RUNNING";

    if (flat == TinyGPS::GPS_INVALID_F_ANGLE) {
      root["LAT"] = 0.0;
    } else {
      root["LAT"] = double_with_n_digits(flat, 6);
    }

    if (flon == TinyGPS::GPS_INVALID_F_ANGLE) {
      root["LON"] = 0.0;
    } else {
      root["LON"] = double_with_n_digits(flon, 6);
    }

    root["ALT"] = gps.altitude();

    if (gps.satellites() == TinyGPS::GPS_INVALID_SATELLITES) {
      root["SAT"] = 0;
    } else {
      root["SAT"] = gps.satellites();
    }

    if (gps.hdop() == TinyGPS::GPS_INVALID_HDOP) {
      root["PREC"] = 0;
    } else {
      root["PREC"] = gps.hdop();
    }

    gps.stats(&chars, &sentences, &failed);
    if (chars == 0){
      root["STATUS"]="FAILURE";
    }

    // Original Serial Outputs
    // Serial.print("LAT=");
    // Serial.print(flat == TinyGPS::GPS_INVALID_F_ANGLE ? 0.0 : flat, 6);
    // Serial.print(" LON=");
    // Serial.print(flon == TinyGPS::GPS_INVALID_F_ANGLE ? 0.0 : flon, 6);
    // Serial.print(" SAT=");
    // Serial.print(gps.satellites() == TinyGPS::GPS_INVALID_SATELLITES ? 0 : gps.satellites());
    // Serial.print(" PREC=");
    // Serial.print(gps.hdop() == TinyGPS::GPS_INVALID_HDOP ? 0 : gps.hdop());

    // root.printTo(Serial);
    // Serial.println();
  }else{
      root["STATUS"] = "IDLE";
      root["LAT"] = 0.0;
      root["LON"] = 0.0;
      root["ALT"] = 0.0;
      root["SAT"] = 0;
      root["PREC"] = 0;
  }
  




  // Original Status Outputs
  // Serial.print(" CHARS=");
  // Serial.print(chars);
  // Serial.print(" SENTENCES=");
  // Serial.print(sentences);
  // Serial.print(" CSUM ERR=");
  // Serial.println(failed);
  // Serial.println();

  // Print out nice JSON to Serial
  // root.prettyPrintTo(Serial);
  root.printTo(xbee);
  xbee.println();
  // Serial.print("Done");

}
