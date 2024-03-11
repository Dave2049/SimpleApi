.PHONY: prep build run register

prep:
	@echo "Checking if 'simple_api' container already exists..."
	@if docker ps -a | grep -q 'simple_api'; then \
		echo "Container 'simple_api' exists. Stopping and removing..."; \
		docker stop simple_api && docker rm simple_api; \
	else \
		echo "Container 'simple_api' does not exist. Proceeding..."; \
	fi

build:
	docker build -t simpleapi:latest .

run:
	docker run -d -p 8080:8080 --network master_default  --name simple_api simpleapi:latest

register:
	docker exec master-nextcloud-1 sudo -u www-data php occ app_api:app:unregister simpleapi --silent --force || true
	docker exec master-nextcloud-1 sudo -u www-data php occ app_api:app:register --json-info "{\"appid\":\"simple_api\",\"name\":\"simple_api\",\"daemon_config_name\":\"manual_install\",\"secret\":\"12345\",\"version\":\"1.0.1\",\"scopes\":[\"\"],\"port\":8080,\"system_app\":0}" --force-scopes --wait-finish simpleapi manual_install


all: prep build run register
	@echo "All done!"