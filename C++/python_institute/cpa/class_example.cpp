#include <iostream>
using namespace std;

class myClass{
    public:
        int value;
        myClass(){
            this->value = 2;
            cout << "it worked" << endl;
        }
};

int main(void)
{
    myClass::myClass();
    myClass object = myClass();
    cout << object.value << endl;
    return (0);
}