FROM alpine

RUN apk --update add vim bash

ENTRYPOINT ["/bin/bash"]

# docker build -t bash-box .
# docker run -dti --name bb bash-box
