int pressureAnalogPin1 = 0; // pin of fsr
int pressureReading1;

int pressureAnalogPin2 = 1; // pin of fsr
int pressureReading2;

int pressureAnalogPin3 = 2; // pin of fsr
int pressureReading3;

int pressureAnalogPin4 = 3; // pin of fsr
int pressureReading4;

int pressureAnalogPin5 = 4; // pin of fsr
int pressureReading5;

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  pressureReading1 = analogRead(pressureAnalogPin1);
  pressureReading2 = analogRead(pressureAnalogPin2);
  pressureReading3 = analogRead(pressureAnalogPin3);
  pressureReading4 = analogRead(pressureAnalogPin4);
  pressureReading5 = analogRead(pressureAnalogPin5);
  
  if (pressureReading1 > 600) {
     Serial.println("HIT1-" + String(pressureReading1));
  }

  if (pressureReading2 > 600) {
     Serial.println("HIT2-" + String(pressureReading2));
  }

  if (pressureReading3 > 600) {
     Serial.println("HIT3-" + String(pressureReading3));
  }

  if (pressureReading4 > 600) {
    Serial.println("HIT4-" + String(pressureReading4));
  }
  
  if (pressureReading5 > 600) {
     Serial.println("HIT5-" + String(pressureReading5));
  }
  delay(10);
}
