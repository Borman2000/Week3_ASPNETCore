# Minikube deployment (split namespaces)

This setup deploys services into two namespaces:

- `apps`: book/auth/notification/gateway + mysql + redis
- `observability`: otel-collector + prometheus + grafana + renderer + jaeger + mysqld-exporter

## 1) Build local images into Minikube Docker daemon

```powershell
minikube start
minikube -p minikube docker-env --shell powershell | Invoke-Expression
docker build -f docker/Book.Dockerfile -t book-service:dev .
docker build -f docker/Auth.Dockerfile -t auth-service:dev .
docker build -f docker/Notification.Dockerfile -t notification-service:dev .
docker build -f docker/Gateway.Dockerfile -t gateway-service:dev .
```

## 2) Apply manifests

Preferred (single command via kustomize):

```powershell
kubectl apply -k kube-config
```

Fallback (explicit apply order):

```powershell
kubectl apply -f kube-config/namespaces.yaml
kubectl apply -f kube-config/apps-configmap.yaml
kubectl apply -f kube-config/apps-secrets.yaml
kubectl apply -f kube-config/mysql-deployment.yaml
kubectl apply -f kube-config/redis-deployment.yaml
kubectl apply -f kube-config/api-deployment.yaml
kubectl apply -f kube-config/auth-service-deployment.yaml
kubectl apply -f kube-config/notification-service-deployment.yaml
kubectl apply -f kube-config/gateway-service-deployment.yaml
kubectl apply -f kube-config/observability-otel-collector.yaml
kubectl apply -f kube-config/observability-mysqld-exporter.yaml
kubectl apply -f kube-config/observability-prometheus.yaml
kubectl apply -f kube-config/observability-grafana.yaml
kubectl apply -f kube-config/observability-jaeger.yaml
```

## 3) Check rollout

```powershell
kubectl get pods -n apps
kubectl get pods -n observability
kubectl get svc -n apps
kubectl get svc -n observability
```

## 4) Access NodePorts

Use `minikube ip` to get the node IP.

- Gateway API: `http://<minikube-ip>:30083`
- Book API: `http://<minikube-ip>:30080`
- Auth API: `http://<minikube-ip>:30081`
- Notification API: `http://<minikube-ip>:30082`
- Prometheus: `http://<minikube-ip>:30090`
- Grafana: `http://<minikube-ip>:30030`
- Jaeger UI: `http://<minikube-ip>:30086`
- MySQL exporter metrics: `http://<minikube-ip>:30104/metrics`

Default Grafana credentials come from `kube-config/observability-grafana.yaml` (`admin` / `admin`).


