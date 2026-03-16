#!/usr/bin/env python3

# ----------------------------------------------------------------------------------------
#
# manager.py
#
# Created to manage add/remove worker operations in Citus docker-compose.
#
# ----------------------------------------------------------------------------------------
import docker
from os import environ
import os
import psycopg2
import signal
from sys import exit, stderr
from time import sleep

# The health check file that is used to signal that the managerr is ready to accept new worker nodes
HEALTHCHECK_FILE= '/healthcheck/manager-ready'

# adds a host to the cluster
def add_worker(conn, host):
    cur = conn.cursor()
    worker_dict = ({'host': host, 'port': 5432})

    print("adding %s" % host, file=stderr)
    cur.execute("""SELECT master_add_node(%(host)s, %(port)s)""", worker_dict)


# removes all placements from a host and removes it from the cluster
def remove_worker(conn, host):
    cur = conn.cursor()
    worker_dict = ({'host': host, 'port': 5432})

    print("removing %s" % host, file=stderr)
    cur.execute("""DELETE FROM pg_dist_placement WHERE groupid = (SELECT groupid FROM 
                   pg_dist_node WHERE nodename = %(host)s AND nodeport = %(port)s LIMIT 1);
                   SELECT master_remove_node(%(host)s, %(port)s)""", worker_dict)


# connect_to_master method is used to connect to master coordinator at the start-up.
# Citus docker-compose has a dependency mapping as worker -> manager -> master.
# This means that whenever manager is created, master is already there, but it may
# not be ready to accept connections. We'll try until we can create a connection.
def connect_to_master():
    citus_host    = environ.get('CITUS_HOST', 'master')
    postgres_pass = environ.get('POSTGRES_PASSWORD', '')
    postgres_user = environ.get('POSTGRES_USER', 'postgres')
    postgres_db   = environ.get('POSTGRES_DB', postgres_user)

    conn = None
    while conn is None:
        try:
            conn = psycopg2.connect("dbname=%s user=%s host=%s password=%s" %
                                    (postgres_db, postgres_user, citus_host, postgres_pass))
        except psycopg2.OperationalError as error:
            print("Could not connect to %s, trying again in 1 second" % citus_host)
            sleep(1)
        except (Exception, psycopg2.Error) as error:
            raise error
        
    conn.autocommit = True

    print("connected to %s" % citus_host, file=stderr)

    return conn

# main logic loop for the manager
def docker_checker():
    client = docker.DockerClient(base_url='unix:///var/run/docker.sock')
    actions = {'health_status: healthy': add_worker, 'destroy': remove_worker}

    # creates the necessary connection to make the sql calls if the master is ready
    conn = connect_to_master()

    # introspect the compose project used by this citus cluster
    my_hostname = environ['HOSTNAME']
    this_container = client.containers.get(my_hostname)
    compose_project = this_container.labels['com.docker.compose.project']

    # we only care about worker container health/destroy events from this cluster
    print("found compose project: %s" % compose_project, file=stderr)
    filters = {'event': list(actions),
               'label': ["com.docker.compose.project=%s" % compose_project,
                         "com.citusdata.role=Worker"],
               'type': 'container'}


    # touch a file to signal we're healthy, then consume events
    print('listening for events...', file=stderr)
    open(HEALTHCHECK_FILE, 'a').close()
    for event in client.events(decode=True, filters=filters):
        worker_name = event['Actor']['Attributes']['name']
        status = event['status']

        status=actions[status](conn, worker_name)


# implemented to make Docker exit faster (it sends sigterm)
def graceful_shutdown(signal, frame):
    print('shutting down...', file=stderr)
    exit(0)


def main():
    if os.path.exists(HEALTHCHECK_FILE):
        os.remove(HEALTHCHECK_FILE)

    signal.signal(signal.SIGTERM, graceful_shutdown)
    docker_checker()


if __name__ == '__main__':
    main()
