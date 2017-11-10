#include <SoftwareSerial.h>
#include <TinyGPS.h>
#include <ArduinoJson.h>
#include <Wire.h>
#include <Servo.h>
/* This sample code demonstrates the normal use of a TinyGPS object.
   It requires the use of SoftwareSerial, and assumes that you have a
   4800-baud serial GPS device hooked up on pins 4(rx) and 3(tx).
*/

//Gimbal
#define CONTROL_DELAY (50)

int read_value=0;
int x_value;
int y_value;

unsigned long x_current;
unsigned long y_current;

// GPS

TinyGPS gps;

SoftwareSerial ss(10, 11);

#define BUFFER_SIZE 200

unsigned long timeStamp;

void setup()
{
  Serial.begin(115200);
  //setup gimbal pins
  pinMode(3, OUTPUT);
  pinMode(4,OUTPUT);
  x_current=millis();
  y_current=millis();
  
  while (!Serial) {
    // wait serial port initialization
  }
  ss.begin(9600);
  while (!ss) {
    // wait serial port initialization
  }
}

void loop()
{
  bool newData = false;
  unsigned long chars;
  unsigned short sentences, failed;

  // For one second we parse GPS data and report some key values
    while (ss.available())
    {
      char c = ss.read();
      
       //Serial.write("LAA"); // uncomment this line if you want to see the GPS data flowing
      if (gps.encode(c)) // Did a new valid sentence come in?
        newData = true;
    }

  timeStamp = millis();

  
  
  if (newData)
  {

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

    root.printTo(Serial);
    Serial.println();
  }
  
  
  // Print out nice JSON to Serial
  // root.prettyPrintTo(Serial);
  //root.printTo(Serial);
  //Serial.println();
  // Serial.print("Done");

  // control gimbal
  while(Serial.available()){
    char temp = Serial.read();
    if(temp=='X'){
      String line = Serial.readStringUntil('\n');
      x_value = line.toInt();
    }
    if(temp=='Y'){
      String line = Serial.readStringUntil('\n');
      y_value = line.toInt();
    }
  }
  
  if(millis()>x_current+CONTROL_DELAY){
    x_current=millis();
        //write gimbal servo
    int tilt=0;
    
    if (x_value<0){
      x_value=0;
    }else if(x_value>90){
      x_value=90;
    }
      tilt=map(x_value, 90,0,1100,1500);
      digitalWrite(3, HIGH);
      delayMicroseconds(tilt);
      digitalWrite(3, LOW);

  //gimbal.write(tilt);
  }
  if(millis()>y_current+CONTROL_DELAY){
    y_current = millis();
    int tilt=0;

    if (y_value<0){
      y_value=0;
    }else if(y_value>180){
      y_value=180;
    }
    tilt=map(y_value, 0,180,800,1800);

  digitalWrite(4, HIGH);
  delayMicroseconds(tilt);
  digitalWrite(4, LOW);
  }


}
