import socket
import sys

HOST = '192.168.1.4'
PORT = 50000

try:
  sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
except socket.error as msg:
  sys.stderr.write("[ERROR] %s\n" % msg[1])
  sys.exit(1)

try:
  sock.connect((HOST, PORT))
except socket.error as msg:
  sys.stderr.write("[ERROR] %s\n" % msg[1])
  sys.exit(2)

msg = '{"id":"1","name":"mahla soleimani","email":"mahla.sol2@gmail.com}'

sock.send(msg.encode('utf-8') )

sock.close()
sys.exit(0)