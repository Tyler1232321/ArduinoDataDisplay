

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

char inputChar = ""; // Char to recieve from the C# code
int analogPin1 = 3; // First analog Pin to transmit Data
int analogPin2 = 4; // Second Analog Pin to transmit Data
int data; // The variable that sends the data
bool tab1 = false; // Wheather Tab one is active
bool tab2 = false; // whether Tab two is active
void loop() {
  // put your main code here, to run repeatedly:
  /*
  while (Serial.available() > 0)
  {
    if (Serial.read() == 83)
    {
      tab1 = true;
    }
    if (Serial.read() == 84)
    {
      tab2 = true;
    }
    if (Serial.read() == 85)
    {
      tab1 = false;
    }
    if (Serial.read() == 86)
    {
      tab2 = false;
    }
  }
  if (tab1)
  {
    Serial.println(analogRead(analogPin1));
    delay(100);
  }
  if (tab2)
  {
    Serial.println(random(10,50));
    delay(100);
  }*/
  
  while (Serial.available() > 0)
  {
    inputChar = (char)Serial.read();
  }
  if (inputChar == 83) 
  {
    tab1 = true;
  }
  if (inputChar == 84)
  {
    tab2 = true;
  }
  if (inputChar == 85)
  {
    tab1 = false;
  }
  if (inputChar == 86)
  {
    tab2 = false;
  }
  if (tab1)
  {
    Serial.print('S');
    delay(50);
    data = analogRead(analogPin1);
    Serial.println(data);
    delay(100);
  }
  if (tab2)
  {
    Serial.print('T');
    delay(50);
    data = random(10,50);
    Serial.println(data);
    delay(100);
  }
}

