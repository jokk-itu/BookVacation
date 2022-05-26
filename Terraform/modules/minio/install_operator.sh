wget https://github.com/minio/operator/releases/download/v4.4.16/kubectl-minio_4.4.16_linux_amd64 -O kubectl-minio
chmod +x kubectl-minio
sudo mv kubectl-minio /usr/local/bin/

kubectl minio version
